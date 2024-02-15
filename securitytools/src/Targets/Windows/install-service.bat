sc create GGolbik.SecurityTools binPath="%~dp0SecurityTools.exe"
sc failure GGolbik.SecurityTools actions= restart/60000/restart/60000/""/60000 reset= 86400
sc start GGolbik.SecurityTools
sc config GGolbik.SecurityTools start=auto
