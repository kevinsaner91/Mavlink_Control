% FMCW-System Evaluation of data recorded with the Radar Config and
% Measurement Tool. Evaluates the linearity
% 15002504 HF Direct Radiating Chip
% W. Mayer, FET, 16.7.2014
% M.Sautermeister, FET, 29.09.2015

clear all
close all
clc

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% USER INPUT 
% fill in here
%%%%%%%%%%%%%%%%%%%%%%%%%%%
TopFolder = 'C:\PathToTopFolder';
maximumSearchRange = [0.5 35]; %in m

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% I N I T
%%%%%%%%%%%%%%%%%%%%%%%%%%%
fmcwevalini_default_RCM;
subfolders = dir(TopFolder);


dispstat('','init'); %one time only init 
dispstat('Begining the process...','keepthis'); 

track_distance = zeros(length(subfolders)-2,1);
eval_distance = zeros(length(subfolders)-2,1);

for i=3:1:length(subfolders)
    folder = [TopFolder '\' subfolders(i).name];

    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % L O A D  M E A S U R E M E N T  D A T A
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    run([folder '\data.m']);
    files = dir([folder '\*.csv']);
    Szf=zeros(length(files), rad.npr);
    for findex = 1:length(files)   
        datafilename = [folder '\data' sprintf('%6.6d',findex) '.csv'];
        ATEMP = csvread(datafilename,1,0);
        Szf(findex, :) = ATEMP';
    end;
    rad.rpc = length(files);

    clear tlist;
    
    [tlist, Sszf, ev] = fmcweval(rad, ev, Szf);

    track_distance(i-2) = 30.5-(rad.dist)/1000;
    eval_distance(i-2) = tlist.r(1);
    
    dispstat(sprintf('Processing %.2f%%',(i-2)*100/(length(subfolders)))); 
end

% figure(1);
% plot(track_distance,eval_distance);
% xlabel('distance of measuring track [m]');
% ylabel('evaluated distance [m]');
% title('Linearity');
% grid on;

%NonLinearity
p=polyfit(track_distance,eval_distance,1);
f=p(1).*track_distance+p(2);
z=f-eval_distance;
display('Non linearity in m:');
max(z)

figure(2);
subplot(2,1,1);
hold off;
plot(track_distance,eval_distance);
%axis([1.25 1.45 1.95 2.07]);
xlabel('distance of measuring track [m]');
ylabel('evaluated distance [m]');
title('Evaluated distances');
grid on;
hold on;
plot(track_distance,f,'Color','red');

subplot(2,1,2);
plot(track_distance,(eval_distance-f)*1000);
xlabel('distance of measuring track [m]');
ylabel('distance error [mm]');
title('Range Linearity');
grid on;

%title
suptitle(['\bf Linearity of module: ' module]);