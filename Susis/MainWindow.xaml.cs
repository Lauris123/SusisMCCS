using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Susis
{
    //abcd
    public partial class MainWindow : Window
    {
        protected static int x = 0;
        protected static int z = 0;

        Dictionary<string, string> funkcijas = new Dictionary<string, string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public static string endl = Environment.NewLine;

        public static string SuperKrutāFunkcijaKuraKompilēKodu(string code)
        {
            string rezultāts = @".586
.MODEL flat, stdcall
include \masm32\include\windows.inc
include \masm32\macros\macros.asm
include \masm32\include\masm32.inc
include \masm32\include\gdi32.inc
include \masm32\include\user32.inc
include \masm32\include\kernel32.inc
includelib\masm32\lib\masm32.lib
includelib\masm32\lib\gdi32.lib
includelib\masm32\lib\user32.lib
includelib\masm32\lib\kernel32.lib
include \masm32\include\msvcrt.inc
includelib \masm32\lib\msvcrt.lib" + Environment.NewLine +
".data"+endl+
"x db \"X\" ,0";
            string[] komandas = code.Split('\n');
            bool VaiIrKods = false;
            foreach (var komanda in komandas)
            {
                string y = komanda.Trim();
                string[] komandasDaļas = y.Split(' ');
                string Nosaukums = komandasDaļas[0];
                if (Nosaukums == "")
                {
                    break;
                }
                if (Nosaukums == "NOSAUKT")
                {
                    if (VaiIrKods == false)
                    {
                        rezultāts += "\r\n" + komandasDaļas[1] + " db ";
                        if (komandasDaļas.Length == 4)
                        {
                            if (komandasDaļas[3] != "")
                            {
                                rezultāts += komandasDaļas[3];
                            }
                            else
                            {
                                rezultāts += "?";
                            }
                        }
                        else {
                            rezultāts += "?";
                        }
                    }
                }
                if (Nosaukums == "Kalkulēt".ToUpper())
                {
                    String metode;
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                        if (komandasDaļas[2] == "+")
                        {
                            metode = "add";
                            rezultāts = Kalkulēt(rezultāts, komandasDaļas, metode);
                        }
                        else if (komandasDaļas[2] == "-")
                        {
                            metode = "sub";
                            rezultāts = Kalkulēt(rezultāts, komandasDaļas, metode);
                        }
                    
                }
                if (Nosaukums == "VAI") 
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    
                    if(komandasDaļas[2] == "!")
                    {
                        rezultāts+="\r\nmov eax, "+ komandasDaļas[1] + Environment.NewLine + "cmp eax, "+ komandasDaļas[3];
                        rezultāts += Environment.NewLine + "je leciens" + x;
                    }
                    else if (komandasDaļas[2] == "=") 
                    {
                        rezultāts += "\r\nmov eax, " + komandasDaļas[1] + Environment.NewLine + "cmp eax, " + komandasDaļas[3];
                        rezultāts += Environment.NewLine + "jne leciens" + x;
                    }
                }
                if(Nosaukums == "NĒ")
                { 
                    rezultāts+="\r\njmp parleciens"+x;
                    rezultāts+="\r\nleciens"+x+":";
                }
                if(Nosaukums == "PĀRSTĀT")
                {
                    rezultāts+="\r\nparleciens"+x+":";
                    x++;
                }
                if (Nosaukums == "ATKĀRTOT") 
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += "\r\natkartojums" + z+":";
                }
                if (Nosaukums == "KAMĒR")
                {
                    rezultāts +=Environment.NewLine+ "mov ecx, "+komandasDaļas[1];
                    rezultāts += Environment.NewLine + "cmp ecx, " + komandasDaļas[3];
                    rezultāts += Environment.NewLine + "je atkartojums" + z;
                    z++;
                }
                if (Nosaukums == "KAMĒRNAV")
                {
                    rezultāts += Environment.NewLine + "mov ecx, " + komandasDaļas[1];
                    rezultāts += Environment.NewLine + "cmp ecx, " + komandasDaļas[3];
                    rezultāts += Environment.NewLine + "jne atkartojums" + z;
                    z++;
                }
                if (Nosaukums == "IZVADĪT")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts = rezultāts + "\r\n mov eax , " + komandasDaļas[1] + "\r\n" + "print str$(eax)";
                }
                if (Nosaukums == "PIEŠĶIRT")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += Environment.NewLine + "mov eax, "+ komandasDaļas[2] + Environment.NewLine + "mov "+komandasDaļas[1]+" , eax";
                }
                if (Nosaukums == "RINDA")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += Environment.NewLine + "print chr$(13,10)";
                }
                if (Nosaukums == "PALIELINĀT")
                {
                    rezultāts += Environment.NewLine + "mov eax , " + komandasDaļas[1] + Environment.NewLine + "inc eax";
                }
                if (Nosaukums == "IELASĪT")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += Environment.NewLine + "mov eax , input()" + Environment.NewLine + "mov " + komandasDaļas[1] + " , eax";
                }
                if (Nosaukums == "IELASĪT4")
                {
                    rezultāts += endl+"mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "and ecx, 00000000000000000000000011111111b"+ Environment.NewLine+
                                 "mov "+komandasDaļas[2]+", ecx" + Environment.NewLine +
                                 "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "shr ecx,8"+ Environment.NewLine+
                                 "and ecx, 00000000000000000000000011111111b"+endl+
                                 "mov "+komandasDaļas[3]+", ecx" + Environment.NewLine +
                                 "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "shr ecx,16"+ Environment.NewLine+
                                 "and ecx, 00000000000000000000000011111111b"+endl+
                                 "mov "+komandasDaļas[4]+", ecx" + Environment.NewLine +
                                 "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "shr ecx,24"+ Environment.NewLine+
                                 "and ecx, 00000000000000000000000011111111b"+endl+
                                 "mov "+komandasDaļas[5]+", ecx" + Environment.NewLine;
                }
                if (Nosaukums == "IZVADĪTČARU") 
                {
                    rezultāts +=endl+"mov ecx,"+komandasDaļas[1]+endl+ "mov x, cl" + endl + "invoke StdOut, addr x";
                }

            }
            if (rezultāts != "")
            {
                rezultāts = rezultāts + @"
mov eax, input (""beigas"")
ret
 main ENDP
END main";
            }

            return rezultāts;
        }

        public static string WriteLines(string[] arr)
        {
            return arr.Aggregate((all, next) => all + Environment.NewLine + next);
        }

        private static string Kalkulēt(string rezultāts, string[] komandasDaļas, string metode)
        {
            if (VaiIrSkaitlis(komandasDaļas[3]) == true)
            {
                rezultāts += Environment.NewLine + "mov eax, " + komandasDaļas[1] + Environment.NewLine + metode + " eax, " +
                    komandasDaļas[3] + Environment.NewLine + "mov " + komandasDaļas[1] + ", eax";
            }
            else
            {
                rezultāts += Environment.NewLine + "mov eax, " + komandasDaļas[1] + Environment.NewLine+"mov ebx , "+komandasDaļas[3]+Environment.NewLine + metode + " eax, ebx" +
                     Environment.NewLine + "mov " + komandasDaļas[1] + ", eax";
            }
            return rezultāts;
        }

       public static bool VaiIrSkaitlis(string s)
        {
            int rezultats;
            try
            {
                rezultats = int.Parse(s);
            }
            catch {
                return false;
            }
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String Code = tbx.Text;



            String asm = SuperKrutāFunkcijaKuraKompilēKodu(Code);

            MessageBox.Show(asm);


            string mape = System.IO.Directory.GetCurrentDirectory();
            string asamblejamisFails = mape + "\\proga.asm";
            string exeFails = "proga.exe";
            string objFails = mape + "\\proga.obj";

            System.IO.File.Delete(asamblejamisFails);
            System.IO.File.Delete(exeFails);
            System.IO.File.Delete(objFails);

            System.IO.File.WriteAllText(asamblejamisFails, asm);



            string strCmdText;
            strCmdText = "/c /coff /Cp /nologo /I\"C:\\Masm32\\Include\" \"" + asamblejamisFails + "\"";
            System.Diagnostics.Process.Start("C:\\masm32\\bin\\ML.EXE", strCmdText);

            System.Threading.Thread.Sleep(2000);

            strCmdText = "/SUBSYSTEM:CONSOLE /RELEASE /VERSION:4.0 /LIBPATH:\"C:\\Masm32\\Lib\" /OUT:\"" + exeFails + "\" \"" + objFails + "\"";
            System.Diagnostics.Process.Start("C:\\masm32\\bin\\LINK.EXE", strCmdText);

            System.Threading.Thread.Sleep(2000);

            try
            {
                System.Diagnostics.Process.Start(exeFails, "");
            }
            catch (Exception)
            {

                MessageBox.Show("Nesakompilējās");
            }
            
            

        }
    }
}
//NOSAUKT X = 1
//IELASĪT X
//IZVADĪT X

//NOSAUKT X = 1
//NOSAUKT Y = 1
//NOSAUKT V
//NOSAUKT Z = 14
//NOSAUKT I = 0
//IZVADĪT X
//RINDA
//IZVADĪT Y
//RINDA
//ATKĀRTOT
//PIEŠĶIRT V X
//KALKULĒT V + Y
//VAI V = 13
//IZVADĪT Z
//NĒ
//IZVADĪT V
//PĀRSTĀT
//RINDA
//PIEŠĶIRT Y X
//PIEŠĶIRT X V
//KALKULĒT I + 1
//KAMĒRNAV I = 10

//.586
//.MODEL flat, stdcall
//include \masm32\include\windows.inc
//include \masm32\macros\macros.asm
//include \masm32\include\masm32.inc
//include \masm32\include\gdi32.inc
//include \masm32\include\user32.inc
//include \masm32\include\kernel32.inc
//includelib\masm32\lib\masm32.lib
//includelib\masm32\lib\gdi32.lib
//includelib\masm32\lib\user32.lib
//includelib\masm32\lib\kernel32.lib
//include \masm32\include\msvcrt.inc
//includelib \masm32\lib\msvcrt.lib
//.data
//X dd 1
//Y dd 1
//V dd ?
//Z dd 14
//I dd 0
// .code
//main PROC
// mov eax , X
//print str$(eax)
//print chr$(13,10)
// mov eax , Y
//print str$(eax)
//print chr$(13,10)
//atkartojums1:
//mov eax, X
//mov V , eax
//mov eax, V
//mov ebx , Y
//add eax, ebx
//mov V, eax
//mov eax, V
//cmp eax, 13
//jne leciens0
// mov eax , Z
//print str$(eax)
//jmp parleciens0
//leciens0:
// mov eax , V
//print str$(eax)
//parleciens0:
//print chr$(13,10)
//mov eax, X
//mov Y , eax
//mov eax, V
//mov X , eax
//mov eax, I
//add eax, 1
//mov I, eax
//mov ecx, I
//cmp ecx, 10
//jne atkartojums1
//mov eax, input ("beigas")
//ret
// main ENDP
//END main


//NOSAUKT A
//NOSAUKT B
//NOSAUKT C
//NOSAUKT D
//NOSAUKT X
//IELASĪT X
//IELASĪT4 X A B C D
//IZVADĪT A
//IZVADĪT B
//IZVADĪT C
//IZVADĪT D