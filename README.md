# 8BitComputer
8BitComputer is a simulation of a programmable 8 bit digital computer based on the work of Ben Eater, in his series [Building an 8-bit breadboar computer](https://www.youtube.com/playlist?list=PLowKtXNTBypGqImE405J2565dvjafglHU).

8BitComputer provides a command line interface for writing and executing 8 bit programs written in assembly language:
![8BitComputer Startup](https://raw.githubusercontent.com/faulknerag/8BitComputer/67c0c826b11a09cc9e2cc728be84c5ba847b9e68/Images/8BitComputer%20Startup.png)

# Menu Options
**[1] Run** Begins executing the program currently stored in RAM  
**[2] RAM** Opens the RAM module to view, edit, or clear the contents of RAM  
**[3] Tick** Executes 1 single tick of the computer clock  
**[4] Settings** Opens the settings module to enable debug and set clock speed  
**[5] Reset** Resets computer state to starting conditions. Does not clear RAM  

**To run the Default Program (Pictured Above)**  
Start 8BitComputer, type '1', and press \<Enter\>. 

Press \<ESC\> at any time to halt execution and return to the main menu. 

**To View the Contents of RAM**  
Use option '2' from the the main menu, then 'D':  
![Memory Module](https://raw.githubusercontent.com/faulknerag/8BitComputer/67c0c826b11a09cc9e2cc728be84c5ba847b9e68/Images/Memory%20Module.png)

**To Clear All Data From RAM**  
Use option '2' from the the main menu, then 'C'. 

**To Write a Custom Program**  
Use option '2' from the the main menu, then type the program line by line using the format  
>    \<Line#\>:\<AssemblyInstruction\>

For example, to set line 2:  
 >    2:01001111  

Enter 'S' to save and return to the main menu. 

See 'Assembly Language' section below for more information about how to write a custom program.

# Assembly Language
8BitComputer can be programmed in a custom assembly language. Each instruction is composed of 8 bits:
* The 4 most significant bits are the instruction number, from 0-15
* The 4 least significant bits are the argument, if one is required. Values from 0 to 15 can be used as arguments

These are the currently implemented assembly language instructions:
| Instruction # | Instruction  | Usage       | Example    | 
| -----------   | -----------  | ----------- | -----------|
| 1 [0001]      | LDA          | Load value stored in RAM at address ARG into Register A | LDA 2: 00010010 |
| 2 [0010]      | ADD          | Add value stored in RAM at address ARG to Register A and store result in Register A | ADD 14: 00101110 |
| 3 [0011]      | SUB          | Subtract value stored in RAM at address ARG from Register A and store result in Register A | SUB 10: 00111010 |
| 4 [0100]      | STA          | Store Register A value in RAM at address ARG | STA 15: 01001111 |
| 5 [0101]      | LDI          | Write ARG to Register A | LDI 3: 01010011 |
| 6 [0110]      | JMP          | Jump to Program Instruction ARG | JMP 6: 01100110 |
| 7 [0111]      | JC           | Jump to Program Instruction ARG if the last sum result in a carry | JC 5: 01110101 |
| 8 [1000]      | JZ           | Jump to Program Instruction ARG if the last sum resulted in the value 0 | JC 1: 10000001 |
| 14 [1110]     | OUT          | Write value of Register A to the display output. No ARG required. | OUT: 11100000 |
| 15 [1111]     | HLT          | Stop all processing. No ARG required. | HLT: 11110000 |

# Sample Programs
Several sample programs are included in the file 'Sample Programs.txt'. 

The default program starts at 1 and repeatedly doubles until overflow is reached, then starts over. These are the raw program bytes:
>    0:01010001  
>    1:11100000  
>    2:01001111  
>    3:00101111  
>    4:10000000  
>    5:11100000  
>    6:01100010  

To intrepret the program:

| # | Raw Bytes   | Instruction  | Argument       | Interpretation    | 
| ----------- | ----------- | -----------  | -----------    | -----------       | 
|0|01010001| 0101 => 5 [LDI] | 0001 => 1 | Write 1 to Register A |
|1|11100000| 1110 => 14 [OUT] | 0000 => 0 | Write value of Register A to the display output |
|2|01001111| 0100 => 4 [STA] | 1111 => 15 | Store Register A value in RAM at address 15 |
|3|00101111| 0010 => 2 [ADD] | 1111 => 15 | Add value stored in RAM at address 15 to Register A and store result in Register A |
|4|10000000| 1000 => 8 [JZ] | 0000 => 0 | Jump to Program Instruction 0 if the last sum resulted in the value 0 |
|5|11100000| 1110 => 14 [OUT] | 0000 => 0 | Write value of Register A to the display output |
|6|01100010| 0110 => 6 [JMP] | 0010 => 2 | Jump to Program Instruction 2 |

# Computer Architecture
8BitComputer is a digital implementation of the physical architecture described in Ben Eater's [original work](https://www.youtube.com/playlist?list=PLowKtXNTBypGqImE405J2565dvjafglHU):
![102021-8-bit CPU control signal overview - YouTube](https://raw.githubusercontent.com/faulknerag/8BitComputer/67c0c826b11a09cc9e2cc728be84c5ba847b9e68/Images/Computer%20Architecture.png)

