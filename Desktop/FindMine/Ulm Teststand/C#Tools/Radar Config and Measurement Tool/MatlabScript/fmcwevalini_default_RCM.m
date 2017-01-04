% Simple FMCW-System Simulation
% Input File
% 15000058 VP E-Band
% W. Mayer, FEW, 28.10.2008
% latest modification 3.12.2009 WiM

% Processing parameters
%the next two will be overwritten by data.m
ev.nfft = 8192;                     % FFT-length (>= rad.npr, obtained by zero padding)
ev.nskip = 100;                     % number of samples to skip at the beginning of the ramp
ev.wtype = 3;                       % window type chose: '0=2=no', '3=taylor', '1=cheby'

ev.wpars = 80;                      % up to three window paramters
ev.normspec = 0;                    % spectrum normalization chose 0=none, 1=max
ev.nmax = 1;                       % number of maxima to detect
ev.mxpr = 1;                        % procedure for spectrum maximum search:
                                    % 1 = amplitude order
                                    % 2 = cell averaging cfar
ev.mxpars =[ maximumSearchRange(1) maximumSearchRange(2) 64 4 1.5 1];       % up to 8 parameters for maximum search
                                    % ev.mxpars(1:2) search range for all procedures:
                                        % mxpars(1) = minimum range in m
                                        % mxpars(2) = maximum range in m
                                    % for mxpr = 1 no further parameters
                                    % for mxpr = 2
                                        % mxpars(3) = single sided CFAR length in cells
                                        % mxpars(4) = protection cells
                                        % mxpars(5) = alpha
                                        % mxpars(6): 0=sum, 1=greatest of, 2=smallest of
ev.rmp = 1;                         % range measurement procedure chose: 
                                    % 1 = dft and successive approximation
                                    % 2 = parable fit
ev.rmpars = [1e-6, 100, 1];         % up to 8 parameters for the range detection procedure 
                                         % for dft: 
                                         % (1) range accuracy to exit iteration process, 
                                         % (2) maxiumum number of iterations 
                                         % (3) input array decimation for DFT calculation.
                                         % for parable fit:
                                         % (1) < 0 dBc-Value to set number of points for fit
                                         % (1) > 0 fixed number of points
                                         % 0<floor(1)<3 3 points are chosen automatically
ev.noisewindow = 200;               % window size vor noise under target calculation
ev.vpr = 1;                         % procedure for velocity processing: 0=none or 1=phase difference
ev.vppars = 124;                      % up to 8 parameters for velocity processing
                                        % vppars(1) = number of doppler cells for ensemble velocity processing
ev.tav = 1;                         % time domain averaging factor (1=no averaging)
ev.sav = 1;                         % spectral domain averaging factor (1=no averaging)

% Range compensation
%ev.rorder =  0.0;					% Order of software range compensation
ev.evalr = 1;						% method for range spectrum calculation
									% 1 = Fast Fourier transform
									% 2 = Welch Method
									% 3 = Fast Fourier transform with autoregressive prediction instead of zero
									%     padding for super-resolution
ev.evalrpars = [0.025 0.5];         % up to 8 parameters for range spectrum calculation
                                    % for ev.evalr = 1 no parameters
                                    % for ev.evalr = 2
                                        % ev.evalrpars(1) = sub-window size relative to whole time sequence
                                        % ev.evalrpars(2) = overlap factor of sub-windows
                                    % for ev.evalr = 3
                                        % ev.evalrpars(1) = order of
                                        %       autoregressive predictor (1..floor(rad.npr/2)-1)
                                        % ev.evalrpars(2) = 1 right sided,
                                        % ev.evalrpars(2) = 2 left sided,
                                        % ev.evalrpars(2) = 3 double sided data extraplation
                                        % data are extrapolated to a total length of ev.nfft
                                        