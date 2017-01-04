clear all;
close all;
clc;

%define window
n=0:4095;

%Cheby 80dB
%w=chebwin(4096,80);

%Cheby TG
%w=csvread('chebyTG.csv');

%Hamming
%w=25/46-21/46*cos(2*pi*n/4095);

%Gauss (sigma=0.4)
%w=exp(-0.5*((n-4095/2)/(0.4*4095/2)).^2);

%Von-Hann
%w=0.5*(1-cos(2*pi*n/4095));

%Rect
%w=ones(4096);

%Rect
w=0.35875-0.48829*cos(2*pi*n/4095)+0.14128*cos(4*pi*n/4095)-0.01168*cos(6*pi*n/4095);

%scale x values from 0 to 1
x=n/n(end);

%plot window
plot(x,w);

p= polyfitn(x(1:2048),w(1:2048),7)

new = polyval(p.Coefficients,x(1:2048));

hold on;

%plot(x,w2);

plot(x(1:2048),new);

p.Coefficients'
