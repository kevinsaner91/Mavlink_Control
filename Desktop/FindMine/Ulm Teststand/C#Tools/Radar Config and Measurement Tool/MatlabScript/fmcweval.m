function [tlist Sszf ev Mszf] = fmcweval(rad, ev, Szf)
% FMCW-System
% Evaluation Function
% 15000058 VP E-Band
% W. Mayer, FEW, 28.10.2008
% latest modification 3.12.2009 WiM
%
% function syntax: [tlist Sszf ev Mszf] = fmcweval(rad, ev, Szf)
%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% global evaluation variables
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
ev.dRmin = c0/(2*rad.df);
rad.ts = 1/rad.fs;  

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% auxiliary variables
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
ts0 = 0:rad.ts:(rad.npr-1)*rad.ts;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% preprocessing
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% trim time vector
ts1 = ts0(ev.nskip+1:rad.npr);
s = length(ts1);

Mszf = Szf(:,ev.nskip+1:rad.npr);
% remove DC
Mmean = repmat(mean(Mszf'), s, 1);
Mszf = Mszf - Mmean';

if ev.evalr == 1
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % FFT processing
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    % set up widow function
    switch ev.wtype
        case 0
            wd = ones(1,s);
        case 1      
            wd = chebwin(s, ev.wpars(1));
            wd = wd';
        case 2
            wd = ones(1,s);
        case 3
            wd = huder(s, ev.wpars(1));
    end    
    
    % apply window
    Mwd = repmat(wd, rad.rpc, 1);
    Mszf = Mszf.*Mwd;

    % calculate fft
    Sszf = fft(Mszf', ev.nfft);
    Sszf = Sszf(1:ev.nfft/2,:);
    if ev.normspec == 1
        Sszf = Sszf/max(max(abs(Sszf)));
    end;
end;

if ev.evalr == 2
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % PSD processing using Welch-method
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    s = floor(ev.evalrpars(1) * ev.nfft);
    nover = floor(ev.evalrpars(2)*s);
    switch ev.wtype
        case 0
            wd = ones(1,s);
        case 1      
            wd = chebwin(s, ev.wpars(1));
            wd = wd';
        case 2
            wd = ones(1,s);
        case 3
            wd = huder(s, ev.wpars(1));
    end    
    
    % calculate power spectral density with Welch-method
    for rl = 1:rad.rpc, 
        [Sszftemp(rl,:),fs] = pwelch(Mszf(rl,:),wd,nover,[],rad.fs,'onesided');   
    end;
    
        % Interpolation to ev.nfft/2 values
    for rl = 1:rad.rpc,
        Sszf(rl,:) = interp(Sszftemp(rl,:),floor(ev.nfft/length(Sszftemp(rl,:))));
    end;
    
    Sszf = Sszf.';
end;

if ev.evalr == 3
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % FFT processing with autoregressive
    % signal extension
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    
    s = ev.nfft;
    
    % set up widow function
    switch ev.wtype
        case 0
            wd = ones(1,s);
        case 1      
            wd = chebwin(s, ev.wpars(1));
            wd = wd';
        case 2
            wd = ones(1,s);
        case 3
            wd = huder(s, ev.wpars(1));
    end    
    
    for rl = 1:rad.rpc,                 % for all ramps per measureement cycle
        % extend signal
        % calculate model coefficients
        A=arburg(Mszf(rl,:),ev.evalrpars(1));

        % right sided signal extension
        if ev.evalrpars(2) == 1
            nright = ev.nfft - length(Mszf(rl,:));      % calculate number of samples to predict
            spred = arforecast(A, Mszf(rl,:), nright);  % perform prediction
            sext = [Mszf(rl,:) spred];                  % combine extended signal
            sext = sext.*wd;                            % apply window function to extended signal
            Sszf(rl,:) = fft(sext, ev.nfft);            % calculate spectrum of extended signal
        % left sided signal extension    
        elseif ev.evalrpars(2) == 2
            nleft = ev.nfft - length(Mszf(rl,:));       % calculate number of samples to predict
            spred = arbackcast(A, Mszf(rl,:), nleft);   % perform prediction
            sext = [spred Mszf(rl,:)];                  % combine extended signal
            sext = sext.*wd;                            % apply window function to extended signal
            Sszf(rl,:) = fft(sext, ev.nfft);            % calculate spectrum of extended signal
        % double sided signal extension
        elseif ev.evalrpars(2) == 3 
            nleft = floor((ev.nfft - length(Mszf(rl,:)))/2);        % calculate left side prediction length
            nright = floor((ev.nfft - length(Mszf(rl,:)))/2);       % calculate right side prediction length
            if mod(ev.nfft - length(Mszf(rl,:)),2) == 1             % if number of original signal samples are
                nright = nright + 1;                                % not equal, add on sample on the right side
            end;
            spredl = arbackcast(A, Mszf(rl,:), nleft);  % perform left sided prediction
            spredr = arforecast(A, Mszf(rl,:), nright); % perform right sided prediction
            sext = [spredl Mszf(rl,:) spredr];          % combine extended signal
            sext = sext.*wd;                            % apply window function to extended signal
            Sszf(rl,:) = fft(sext, ev.nfft);            % calculate spectrum of extended signal
        end;
    end; 
    
    % post processing
    % adopt matrix orientation
    Sszf = Sszf.';
    % truncate to positve frequencies
    Sszf = Sszf(1:ev.nfft/2,:);
    % optinal amplitude normalization
    if ev.normspec == 1
        Sszf = Sszf/max(max(abs(Sszf)));
    end;
end;

% calculate range vector
ev.fbin = rad.fs / ev.nfft;
ev.rbin = ev.fbin * c0 /(2*rad.S);
ev.Rs = (0:length(Sszf)-1)*ev.rbin;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% postprocessing e. g. normalization
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% normalize results to maximum
if ev.normspec == 1
    Sszf = Sszf/max(max(abs(Sszf)));
end;
Sszfamp = abs(Sszf);
Sszfph = angle(Sszf);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Detection Process
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[tlist] = fmcweval_maxima_detection(rad, ev, Sszfamp);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% range measurement procedure
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[tlist] = fmcweval_range(rad, ev, tlist, ts1, Mszf, Sszf);

 %dim_tlist = size(tlist.i);
 %for ml = 1:dim_tlist(2),                % for all detected targets
 %    for rl = 1:rad.rpc,                 % for all ramps per measureement cycle
 %        if tlist.i(rl,ml) > 1 && tlist.i(rl,ml) <  ev.nfft/2
 %                fm = (tlist.i(rl,ml)-1) * ev.fbin; 
 %                tlist.r(rl,ml) = fm/2/rad.S*c0;
 %        end
 %    end
 %end

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% velocity measurement procedure
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%[tlist] = fmcweval_velocity(rad, ev, tlist, ts1, Mszf);






