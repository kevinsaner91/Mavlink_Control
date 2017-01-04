% FMCW-System Evaluation of data recorded with the Radar Config and
% Measurement Tool
% 15002504 HF Direct Radiating Chip
% W. Mayer, FET, 16.7.2014
% M.Sautermeister, FET, 24.06.2015

clear all
close all
clc

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% USER INPUT 
% fill in here
%%%%%%%%%%%%%%%%%%%%%%%%%%%
testpath = 'C:\PathToCSVfiles';
maximumSearchRange = [0.5 4]; %in m

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% I N I T
%%%%%%%%%%%%%%%%%%%%%%%%%%%
files = dir([testpath '\*.csv']);
fmcwevalini_default_RCM;
run([testpath '\data.m']);
ev.timestamps = zeros(1,length(files));

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% L O A D  M E A S U R E M E N T  D A T A
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

Szf=zeros(length(files), rad.npr);
for findex = 1:length(files)   
    datafilename = [testpath '\data' sprintf('%6.6d',findex) '.csv'];
    ATEMP = csvread(datafilename,1,0);
    Szf(findex, :) = ATEMP';
end;
rad.rpc = length(files);
    
%%%%%%%%%%%%%%%%%%%%%%%%%%%
% E V A L U A T E
%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% ramp by ramp evaluation
[tlist Sszf ev] = fmcweval(rad, ev, Szf);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% ensemble evaluation
[MVdB RD TLS] = fmcweval_ensemble(rad, ev, Sszf, tlist);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% output diagrams
fs = (1:length(Sszf))*ev.fbin;
Rs = ev.Rs;

figure('units','normalized','outerposition',[0 0 1 1])

subplot(8,2,[1,3,5,7]);
plot(Rs, 20*log10(abs(Sszf)));

xlabel('R /m');
ylabel('dB');
title('Huellkurve');
grid on;

subplot(8,2,[2,4,6,8]);
plot(Rs, MVdB(1,:), Rs, MVdB(2,:), Rs, MVdB(3,:));
axis([0 15 30 120]);
xlabel('R /m');
ylabel('dB');
title('Huellkurve');
legend('Mittelwert','Betragsmittelwert','Standardabweichung');
grid on;

subplot(8,2,9);
tn=1;
plot(tlist.r(:,tn),'-o');
title('tlist.r');
ylabel('R in m');
subplot(8,2,11);
plot(unwrap(angle(tlist.a(:,tn))),'-o');
title('tlist.ph');
ylabel('phase in deg');
subplot(8,2,13);
plot(tlist.i(:,tn),'-o');
title('tlist.i');
ylabel('bin');
subplot(8,2,15);
plot(abs(tlist.a(:,tn)),'-o');
title('tlist.a');
ylabel('amplitude');
xlabel('a');

subplot(8,2,[10,12,14,16]);
polar(angle(tlist.a/max(abs(tlist.a))),abs(tlist.a/max(abs(tlist.a))),'x');

%title
suptitle(['\bf Module: ' module ' (r_{std}: ' num2str(TLS.rstd*1000) ' mm)']);