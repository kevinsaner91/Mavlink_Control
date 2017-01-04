function S = dft_us(s,t,f,usf)
% Spectral analysis using the dft of a time domain signal
% with time samples s at times t. The analysis is done 
% for frequencies in the vector f. With usf>=1 the time
% domain signal is undersampled, meaning, that for usf=2,3..
% only every second, third .. time domain sample ist used for
% the DFT calculation.

%S = dft_ue(s,t,f,usf)

for il = 1:length(f),
   S(il) = sum(s(1:usf:end).*exp(-j*2*pi*f(il).*t(1:usf:end)));
end;

%N = length(s);
%k=f/1.5259e+03;

%Alpha = 2*pi*k/N;
%Beta = 2*pi*k*(N-1)/N;
  
% Precompute network coefficients
%Two_cos_Alpha = 2*cos(Alpha);
%a = cos(Beta);
%b = -sin(Beta);
%c = sin(Alpha)*sin(Beta) -cos(Alpha)*cos(Beta);
%d = sin(2*pi*k);
  
% Init. delay line contents
%w1 = 0;
%w2 = 0;
  
%for n = 1:N % Start the N-sample feedback looping
%    w0 = s(n) + Two_cos_Alpha*w1 -w2;
%    % Delay line data shifting
%      w2 = w1;
%      w1 = w0;
%end
  
%S = w1*a + w2*c + 1j*(w1*b +w2*d);

% N = length(s);
% k = f/1.5259e+03 + 1;
% 
% %Precalculation of constants
% A = 2* pi *k / N;
% B = 2 * cos(A);
% C = exp(-j*A);
% D = exp(-j*2* pi *k / N *(N-1));
% %State variables
% s0 = 0;
% s1 = 0;
% s2 = 0;
% %Main loop
% for i=1:N-1 %one iteration less than traditionally
%     s0 = s(i)+B * s1 - s2; %(16)
%     s2 = s1;
%     s1 = s0;
% end
% %Finalizing calculations
% s0 = s(N-1)+B*s1 - s2; %corresponds to (16)
% S = s0 - s1 * C;
% S = S*D; %constant substituting the iteration N ?1, and correcting the phase at the same time

    