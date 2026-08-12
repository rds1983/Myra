### Overview
This tutorial demonstrates how to edit a stylesheet using MyraPad.

### Tutorial
1. Download and unpack the file: [stylesheetEdit.zip](~/files/stylesheetEdit.zip). 

   It consists of a UI in MML (allControls.xmmp) and a linked stylesheet (which is a copy of the Myra default stylesheet).
2. Open allControls.xmmp in [MyraPad](MyraPad.md):

![alt text](~/images/editing-stylesheet-using-myrapad1.png)

3. Open stylesheet/default_ui_skin.xmms in any text editor.
4. Now let's make the button background light coral. 

   Find the following line:
   ```xml
     <ButtonStyle Background="button" OverBackground="button-over" PressedBackground="button-down" />
   ```
   And replace it with:   
   ```xml
     <ButtonStyle Background="#F08080" OverBackground="button-over" PressedBackground="button-down" />
   ```
   Save stylesheet/default_ui_skin.xml.
5. Now, in MyraPad, click File/Reload or press Ctrl+R.

   Observe that the button backgrounds have become light coral:

![alt text](~/images/editing-stylesheet-using-myrapad2.png)

