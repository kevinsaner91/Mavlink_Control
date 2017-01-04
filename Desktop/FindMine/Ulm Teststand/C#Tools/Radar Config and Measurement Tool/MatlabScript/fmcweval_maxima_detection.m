function [tlist] = fmcweval_maxima_detection(rad, ev, Sszfamp)
% Sub-routine of fmcweval for maxiam detection in a radar response
% 
% Input parameters:
% rad: radar paramter set
% ev: evaluation parameter set
% Sszfamp: range envelope curves
% 
% Output parameters:
% tlist: target list with indices (tlist.i) of detected targets
%
% 15000058 VP E-Band
% W. Mayer, FEW, 28.10.2008
% latest modification 3.12.2009 WiM
% 
% syntax: [tlist] = fmcweval_maxima_detection(rad, ev, Sszfamp)

% reserve maximum target list variables
tlist.r = zeros(rad.rpc, ev.nmax); % range list
tlist.v = zeros(rad.rpc, ev.nmax); % radial velocity list
tlist.a = zeros(rad.rpc, ev.nmax); % complex amplitude
tlist.i = zeros(rad.rpc, ev.nmax); % bin number

% estimate expected 3dB PTR-width in bins
nbinsPTR = floor(ev.dRmin/ev.rbin);
% minimum PTR-width 5 bins
if nbinsPTR < 5,
   nbinsPTR = 5;
end;


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% maxima detection process
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% search range can be limited by 
% ev.mxpars(1) = minimum range in m
% ev.mxpars(2) = maximum range in m.
binmin = round(ev.mxpars(1)/ev.rbin);
binmax = round(ev.mxpars(2)/ev.rbin);

if(binmin<1)
    binmin=1;
elseif(binmin>ev.nfft/2)
    binmin=ev.nfft/2;
end;

if(binmax<1)
    binmax=1;
elseif(binmax>ev.nfft/2)
    binmax=ev.nfft/2;
end;

dummyamp = Sszfamp;

switch ev.mxpr
    % simple amplitude order detection process
    case 1 % detection by amplitude order
        for rl = 1:rad.rpc
            for ml = 1:ev.nmax,
                [ma, mi] = max(abs(dummyamp(binmin:binmax,rl)));
                mi = mi + binmin - 1;
                tlist.a(rl,ml) = ma;
                tlist.i(rl,ml) = mi;
                if mi > nbinsPTR/2+1
                    dummyamp(mi-floor(nbinsPTR/2):mi+floor(nbinsPTR/2),rl) = dummyamp(mi-floor(nbinsPTR/2):mi+floor(nbinsPTR/2),rl)* 0;
                else
                    dummyamp(mi:mi+floor(nbinsPTR/2),rl) = dummyamp(mi:mi+floor(nbinsPTR/2),rl)* 0;
                end;
            end; 
        end;
    case 2 % detection by cell averaging CFAR    
        % ev.mxpars(3) = single sided CFAR length in cells
        % ev.mxpars(4) = protection cells
        % ev.mxpars(5) = alpha
        % ev. mxpars(6): 0=sum, 1=greatest of
        % prepare local cfar variables
            for rl = 1:rad.rpc     
                        ml = 1;
               for ic = binmin:1:binmax
                        % check upper range limit
                        if (ic+ev.mxpars(3)+ev.mxpars(4)) > length(Sszfamp(:,rl))
                            break;
                        end;
                        % make average cell sums
                        sumright = sum(Sszfamp((ic+ev.mxpars(4)):(ic+ev.mxpars(3)+ev.mxpars(4)),rl))/ev.mxpars(3);
                        
                        % exception handling for lower range limit
                        if (binmin - ev.mxpars(3) - ev.mxpars(4)) < 1
                           sumleft = sumright;
                        else
                           sumleft =  sum(Sszfamp((ic-ev.mxpars(3)-ev.mxpars(4)):(ic-ev.mxpars(4)),rl))/ev.mxpars(3);
                        end;
                        % handle greatest of / smallest of options
                        if ev.mxpars(6) == 1
                            checksum = 2*max([sumleft sumright]);
                        elseif ev.mxpars(6) == 0
                            checksum = sumleft + sumright;
                        elseif ev.mxpars(6) == 2
                            checksum = 2*min([sumleft sumright]);
                        end;
                        % make decision
                        if Sszfamp(ic,rl) > (ev.mxpars(5)*checksum)
                           tlist.a(rl,ml) = Sszfamp(ic,rl);
                           tlist.i(rl,ml) = ic;

                           % exception handling for neighboring maxima
                           % if maxima distance lower one resolution cell, no new target struct is
                           % generated and the previous target is rewritten if the new target's amplitude 
                           % is higher than the previous ones.
                           
                           if  ((ml>1) && (tlist.i(rl,ml)-tlist.i(rl,ml-1)) <= nbinsPTR)
                               if  abs(tlist.a(rl,ml)) > abs(tlist.a(rl,ml-1))
                                   tlist.a(rl,ml-1) = tlist.a(rl,ml);
                                   tlist.i(rl,ml-1) = tlist.i(rl,ml);
                               end;
                               tlist.a(rl,ml) = 0;
                               tlist.i(rl,ml) = 0;
                           else
                                ml = ml + 1;
                           end;
                        end;
                        if ml==(ev.nmax+1)
                            break;
                        end;
                end; % end of envelope curve
            end; % end of rad.rpc-loop
    
    case 3 %FAC not rising
        for rl = 1:rad.rpc
            FAC(binmin-1) = Inf;
            ml=1;
            for ic = binmin:binmax
                if ic == binmin
                    if (ic-ev.mxpars(3))<=0
                        FACsum = abs((ic-ev.mxpars(3)+1))*Sszfamp(1,rl);
                        FACsum = FACsum + sum(Sszfamp(1:(ic+ev.mxpars(3)),rl));
                    else
                        FACsum = sum(Sszfamp((ic-ev.mxpars(3)):(ic+ev.mxpars(3)),rl));
                    end
                else
                    if (ic-ev.mxpars(3))<=0
                        FACsum = FACsum - Sszfamp(1,rl);
                    else
                        FACsum = FACsum - Sszfamp(ic-ev.mxpars(3),rl);
                    end
                    FACsum = FACsum + Sszfamp((ic+ev.mxpars(3)),rl);
                end
                
                if ((FACsum/(2*ev.mxpars(3)+1))<FAC(ic-1))
                    FAC(ic) = FACsum/(2*ev.mxpars(3)+1);
                else
                    FAC(ic) = FAC(ic-1);
                end
                
                % make decision
                if Sszfamp(ic,rl) > (ev.mxpars(5)*FAC(ic))
                   tlist.a(rl,ml) = Sszfamp(ic,rl);
                   tlist.i(rl,ml) = ic;

                   % exception handling for neighboring maxima
                   % if maxima distance lower one resolution cell, no new target struct is
                   % generated and the previous target is rewritten if the new target's amplitude 
                   % is higher than the previous ones.

                   if  ((ml>1) && (tlist.i(rl,ml)-tlist.i(rl,ml-1)) <= nbinsPTR)
                       if  abs(tlist.a(rl,ml)) > abs(tlist.a(rl,ml-1))
                           tlist.a(rl,ml-1) = tlist.a(rl,ml);
                           tlist.i(rl,ml-1) = tlist.i(rl,ml);
                       end;
                       tlist.a(rl,ml) = 0;
                       tlist.i(rl,ml) = 0;
                   else
                        ml = ml + 1;
                   end;
                end;
                
                if ml>ev.nmax
                    break;
                end;
                
            end; 
        end;
end;

