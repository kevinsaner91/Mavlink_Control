function [MVdB RD TLS] = fmcweval_ensemble(rad, ev, Sszf, tlist)
% Sub-routine for ensemble evaluations of a series of ramps
%
% Input parameters:
% rad: radar paramter set
% ev: evaluation parameter set
% Sszf: complex range respones = complex envelope curves 
% 
% Output parameters:
% MV: complex and amplitude mean and variance curve of range responses
% RD: range and doppler diagram and velocity vector
% TLS: mean value and standard deviation of the target lists
%
% 15000058 VP E-Band
% W. Mayer, FEW, 4.11.2009
% 
% syntax: [MV RD TLS] = fmcweval_ensemble(rad, ev, Sszf)

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% complex mean value,
% amplitude mean value
% complex variance,
%%%%%%%%%%%%%%%%%%%%%%%%%%%
MVdB(1,:) = 20*log10(abs(mean(Sszf,2)));
MVdB(2,:) = 20*log10(mean(abs(Sszf),2));
MVdB(3,:) = 10*log10(abs(var(Sszf,1,2)));

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% range-doppler-diagram
%%%%%%%%%%%%%%%%%%%%%%%%%%%
dwin = huder(rad.rpc, 40);
Dwin = repmat(dwin,length(Sszf(:,1)),1);
RDspectrum = fftshift(fft(Sszf.*Dwin,ev.vppars(1),2),2);
fm = rad.f0 + rad.S * rad.T/2;
dts = ev.timestamps(2)-ev.timestamps(1);                % doppler sampling time
dT = ev.vppars(1)* dts;                                 % doppler observing time
dfd = 1/dT;                                             % doppler frequency resolution
dv = dfd/fm * c0/2;                                     % velocity resolution
vvector = (-ev.vppars(1)/2)*dv:dv:(ev.vppars(1)/2-1)*dv;  % velocity vector
RD.results = RDspectrum;
[l b] = size(RDspectrum);
RD.x = repmat(vvector,l,1);
RD.y = repmat(ev.Rs, b, 1);
RD.y = RD.y';

%%%%%%%%%%%%%%%%%%%%%%%%%%
% target list statistics
%%%%%%%%%%%%%%%%%%%%%%%%%%
TLS.rstd = std(tlist.r, 0, 1);
TLS.rmean = mean(tlist.r, 1);
TLS.astd = std(tlist.a, 0, 1);
TLS.amean = mean(tlist.a, 1);
TLS.vstd = std(tlist.v, 0, 1);
TLS.vmean = mean(tlist.v, 1);





