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
maximumSearchRange = [0.5 6]; %in m

%Reference
use_reference_data = 1;
ReferenceCSV = '00_reference.csv';

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
% L O A D  R E F E R E N C E  D A T A
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
if use_reference_data == 1
    display('Read reference data...');
    fid = fopen([TopFolder '\' ReferenceCSV]);

    tline = fgetl(fid); %Header
    tline = fgetl(fid);
    idx=1;
    while ischar(tline)
        tlineArray = strsplit(tline,';');
        ref_time(idx) = datetime([tlineArray{1} ' ' tlineArray{2}],'InputFormat','dd.MM.yyyy HH:mm:ss');
        ref_distance_tank1(idx) = str2double(strrep(tlineArray(3), ',', '.'))/1000.0;
        ref_distance_tank2(idx) = str2double(strrep(tlineArray(4), ',', '.'))/1000.0;
        tline = fgetl(fid);
        idx = idx+1;
    end

    fclose(fid);
    display('Done!');
    fprintf('\n');
end;

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
if use_reference_data == 1
    plot(eval_time, eval_distance, ref_time ,ref_distance_tank2);
    legend('Radar', 'Reference');
else
    plot(eval_time, eval_distance);
end;
xlabel('Time');
ylabel('distance [m]');
title('Distance over Time');
grid on;

%Amplitude over Distance
figure(2);
plot(eval_distance,20*log10(eval_amplitude));
ylabel('Amplitude[dB]');
xlabel('evaluated distance [m]');
title('Amplitude over Distance');
grid on;

% Distance Comparison
% daytime to second conversion
figure(4);
rft = 3600*hour(ref_time)+60*minute(ref_time)+second(ref_time);
rft = rft-min(rft);
evt = 3600*hour(eval_time)+60*minute(eval_time)+second(eval_time);
evt = evt-min(evt);
rfd = ref_distance_tank2;
evd = resample(eval_distance, length(ref_time), length(eval_time));
evd = transpose(evd);
plot(rft, evd-rfd);
grid on;
xlabel('seconds');
title('distance: radar - tankref');
ylabel('m');


