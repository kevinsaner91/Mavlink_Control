# Mavlink_Control - FindMine

This repository shows how to get a phytec mira board sending and receiving messages to a pixhawk autopilot. Further documentation is shown in the doc directory.

Hereby the mavlink_control directory contains the code needed for the FindMine project. The mavlink_control directroy was edited in Eclipse and is thus ready to be imported into the Eclipse C/C++ IDE. In order to make the code running, it is neccessary that the lib directory is included into the Eclipse project.

The program is currently setup to receive the GPS-Data including the timestamp (since system boot) of the pixhawk. If data is available it is written to a csv-file.
