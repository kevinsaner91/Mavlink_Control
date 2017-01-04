/** Konfiguriert den motor auf absolut positionierung
* und fährt zwischen 2 positionen mit verschiedenen geschwindigkeiten
* hin und her
* */
import nanotec.*;

class ConfigDriveExample {
	public static void main() {
		//Motor konfigurieren
		drive.SetMode(1); //Absolut Positionierung
		drive.SetMinSpeed(100);
		drive.SetAcceleration(2000); //Rampe
		drive.SetCurrent(10); //Strom
		drive.SetCurrentReduction(1);//Strom für Reduzierung
		util.SetStepMode(2); //1/2 Schritt modus
		drive.SetPosition(0);
		capture.SetCaptTime(50);
		capture.SetCaptiPos(1);
		//Hauptschleife
		for(int i=0; i < 2; i++){
			drive.SetDirection(0);
			drive.SetMaxSpeed(1000); //Geschwindigkeit
			drive.SetTargetPos(12870); //Ziel
			drive.StartDrive( );
			
			util.Sleep(2000); //2 Sekunde warten
			
		}
		//~ for(int j=0; j < 2; j++){
			//~ drive.SetDirection(1);
			//~ drive.SetMaxSpeed(1000); //Geschwindigkeit
			//~ drive.SetTargetPos(12870); //Ziel
			//~ drive.StartDrive( );
			
			//~ util.Sleep(2000); //2 Sekunden warten
		//~ }
	}
}