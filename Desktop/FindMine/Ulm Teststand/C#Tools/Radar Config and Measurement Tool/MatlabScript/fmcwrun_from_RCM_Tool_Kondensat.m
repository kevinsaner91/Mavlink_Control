% FMCW-System Evaluation of data recorded with the Radar Config and
% Measurement Tool. Evaluates the linearity
% 15002504 HF Direct Radiating Chip
% W. Mayer, FET, 16.7.2014
% M.Sautermeister, FET, 29.09.2015
% W. Mayer, TTD, 03.02.2016

clear all
close all
clc

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% USER INPUT 
% fill in here
%%%%%%%%%%%%%%%%%%%%%%%%%%%
TopFolder = 'C:\PathToCSVfiles';
maximumSearchRange = [0.5 8]; %in m
% Envelope Movie
make_envelope_movie = 1;    

%%%%%%%%%%%%%%%%%%%%%%%%%%%
% I N I T
%%%%%%%%%%%%%%%%%%%%%%%%%%%

if make_envelope_movie == 1
    moviefilename = [TopFolder '\' 'EnvelopeMovie.gif'];
    fps=50;
    moviefigure = 3;
    moviescale = [0 6 30 130];
    moviescale_distance = [0 1 0 10];
    moviescale_sn = [0 1 50 150];
    mfh=figure(moviefigure);
    set(mfh, 'Units', 'normalized', 'Position', [0.1, 0.1, 0.5, 0.8]);
    set(mfh,'Color',[1,1,1]);
end;

fmcwevalini_default_RCM;
subfolders = dir(TopFolder); 


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% L O A D  M E A S U R E M E N T  D A T A
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
dispstat('','init'); %one time only init 
dispstat('Load and evaluate measurement data...','keepthis');

run([TopFolder '\data.m']);
files = dir([TopFolder '\data*.csv']);
Szf=zeros(length(files), rad.npr);

eval_distance = zeros(length(files),1);
eval_noise = zeros(length(files),1);
eval_amplitude = zeros(length(files),1);
eval_index = 1:length(files);

for findex = 1:length(files)
    datafilename = [TopFolder '\data' sprintf('%6.6d',findex) '.csv'];
    ATEMP = csvread(datafilename,1,0);
    out=textread(datafilename, '%s', 'whitespace',',');
    dateAndTime = strsplit(out{1},'Creation: ');
    [tlist, Sszf, ev] = fmcweval(rad, ev, ATEMP');
    
    eval_time(findex) = datetime(dateAndTime(2),'InputFormat','dd.MM.yy HH:mm:ss');
    eval_distance(findex) = tlist.r(1);
    eval_amplitude(findex) = abs(tlist.a(1));
    eval_noise(findex) = tlist.n(1);
    
    dispstat(sprintf('Processing %.2f%%',(findex)*100/(length(files))));
    
    if make_envelope_movie == 1
            % envelope
            subplot(3,1,1);
            evplot=plot(ev.Rs, 20*log10(abs(Sszf)),eval_distance(findex), 20*log10(eval_amplitude(findex)),'o');
            axis(moviescale);
            grid on;
            xlabel('Distance /m');
            ylabel('dB');
            title(['Time: ' dateAndTime(2)]);
            % distance
            subplot(3,1,2);
            plot(eval_index(1:findex), eval_distance(1:findex));
            %moviescale_distance(2) = length(files);
            %axis(moviescale_distance);
            grid on;
            xlabel('index');
            ylabel('Distance /m');
            %drawnow;
            % amplitude
            subplot(3,1,3);
            plot(eval_index(1:findex), 20*log10(eval_amplitude(1:findex)+1e-12), eval_index(1:findex),20*log10(eval_noise(1:findex)+1e-12));
            %moviescale_sn(2) = length(files);
            %axis(moviescale_sn);
            grid on;   
            xlabel('index');
            ylabel('dB');
            legend('target level','noise level');
            drawnow;
            frame = getframe(moviefigure);
            im = frame2im(frame);
            [imind,cm] = rgb2ind(im,256);
            if findex == 1;
                imwrite(imind,cm,moviefilename,'gif', 'Loopcount',inf,'delaytime',1/fps);
            else
                imwrite(imind,cm,moviefilename,'gif','WriteMode','append','delaytime',1/fps);
            end; 
    end;
end;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% P L O T  D A T A
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
figure(1);
plot(eval_time, eval_distance);
xlabel('Time');
ylabel('distance [m]');
title('Distance over Time');
legend('Radar');
grid on;

%Amplitude over Distance
figure(2);
plot(eval_time,20*log10(eval_amplitude));
ylabel('Amplitude[dB]');
xlabel('Time');
title('Amplitude');
grid on;