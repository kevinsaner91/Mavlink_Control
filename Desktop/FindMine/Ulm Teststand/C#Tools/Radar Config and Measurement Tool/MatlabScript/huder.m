function [a]=huder(anz,rn)
% Berechnet eine Belegungsfunktion für Nebenkeulen
% mit maximalem Abstand rn
% anz = Anzahl Punkte (Integer)
% rn = erwünschter Nebenkeulenabstand /dB
%
rz = 0;
a=ones(1,anz);

if rn >= 13.26144
   if rn >= 13.26144 b=-0.009; end;
   if rn >= 15. b=0.34; end;
	if rn >= 20. b=0.72; end;
	if rn >= 25. b=1.01; end;
	if rn >= 30. b=1.24; end;
   if rn >= 35. b=1.50; end;
   if rn >= 40; b=1.73; end;
	while rz <= rn,
		rz = 20 * log10( 4.60333 * sinh(pi * b) / (pi*b));
		b = b + 0.01;
	end;
end
%
if rn < 13.26144
	if rn <= 13.26144 b=-0.441; end;
	if rn <= 10. b=-0.670; end;
	if rn <= 5. b=-0.80; end;
	while rz <= rn,
		rz = 20 * log10( 4.60333 * sin(pi * b) / (pi*b));
        b = b + 0.01;
   end;
end
%
tmax = 0;
for il=1:anz
		x = -1. + 2. * (il-1) / (anz - 1);
		arg = pi * b * sqrt(1. - x^2);
		if rn >= 13.26144  t = besseli(0,arg);  end;
		if rn < 13.26144   t = besselj(0,-arg); end;
		a(il) = a(il) / abs(a(il)) * t;
		if t > tmax tmax = t; end;
end
for il = 1:anz; a(il) = a(il) / tmax; end;


