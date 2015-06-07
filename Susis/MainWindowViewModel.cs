using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Susis
{
    class MainWindowViewModel : BindableBase
    {
        #region "Datu lauki"

        int _ifJumpNumber = 0;
        int _whileJumpNumber = 0;
        string _endl = Environment.NewLine;

        #endregion

        #region "Bindingi"

        private string _input;
        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                this.SetProperty(ref _input, value);
            }
        }

        private string _output;
        public string Output
        {
            get
            {
                return _output;
            }
            set
            {
                this.SetProperty(ref _output, value);
            }
        }

        #endregion

        #region "Komandas"

        public DelegateCommand _compileCommand;
        public DelegateCommand CompileCommand
        {
            get
            {
                return this._compileCommand;
            }
        }

        public DelegateCommand _runCommand;
        public DelegateCommand RunCommand
        {
            get
            {
                return this._runCommand;
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            this._compileCommand = new DelegateCommand(ExecuteCompileCommand);
            this._runCommand = new DelegateCommand(ExecuteRunCommand);

            _input = @"NOSAUKT X = 3
IZVADĪT X
RINDA
IZVADĪT X
IELASĪT X
";
        }

        #region "Privātās metodes"
        string Print(string printValue)
        {
            return "\r\n mov eax , " + printValue + "\r\n" + "print str$(eax)";    
        }

        #region "Komandu metodes"

        void ExecuteRunCommand()
        {
            if (string.IsNullOrEmpty(Output))
                return;

            string mape = System.IO.Directory.GetCurrentDirectory();
            string asamblejamisFails = mape + "\\proga.asm";
            string exeFails = "proga.exe";
            string objFails = mape + "\\proga.obj";

            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
                if (process.ProcessName.Contains("proga"))
                    process.Kill();

            Thread.Sleep(2000);

            System.IO.File.Delete(asamblejamisFails);
            System.IO.File.Delete(exeFails);
            System.IO.File.Delete(objFails);

            System.IO.File.WriteAllText(asamblejamisFails, Output);



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

        void ExecuteCompileCommand()
        {
            Output = CompileCode(Input);
        }

        #endregion

        string Calculate(string rezultāts, string[] komandasDaļas, string metode)
        {
            if (IsNumber(komandasDaļas[3]) == true)
            {
                rezultāts += Environment.NewLine + "mov eax, " + komandasDaļas[1] + Environment.NewLine + metode + " eax, " +
                    komandasDaļas[3] + Environment.NewLine + "mov " + komandasDaļas[1] + ", eax";
            }
            else
            {
                rezultāts += Environment.NewLine + "mov eax, " + komandasDaļas[1] + Environment.NewLine + "mov ebx , " + komandasDaļas[3] + Environment.NewLine + metode + " eax, ebx" +
                     Environment.NewLine + "mov " + komandasDaļas[1] + ", eax";
            }
            return rezultāts;
        }

        string CompileCode(string code)
        {
            string rezultāts = ".586" + _endl +
@".MODEL flat, stdcall
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
".data" + _endl +
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
                        if (komandasDaļas[1] == "TEKSTS")
                        {
                            string s = "";
                            for (int i = 5; i < komandasDaļas.Length - 1; i++)
                                s += komandasDaļas[i] + " ";
                            s = s.Trim();
                            rezultāts += _endl + komandasDaļas[2] + " db \"" + s + "\",0";


                        }
                        else
                        {

                            rezultāts += "\r\n";
                            if (komandasDaļas.Length == 4)
                            {
                                if (komandasDaļas[3] != "")
                                {
                                    rezultāts += komandasDaļas[1] + " dd ";
                                    rezultāts += komandasDaļas[3];
                                }
                                else
                                {
                                    rezultāts += "?";
                                }
                            }
                            else
                            {
                                string rezultātskompilācijai = CallBuiltInCompiler("nskt " + komandasDaļas[1]);
                                if (rezultātskompilācijai == "err nskt")
                                {
                                    MessageBox.Show("Kļūda kompilātorā err nskt");
                                    return "";
                                }
                                rezultāts += rezultātskompilācijai;
                            }
                        }
                    }

                }
                if (Nosaukums == "TEKSTS")
                {
                    if (VaiIrKods == false)
                    {
                        string s = "";
                        for (int i = 3; i < komandasDaļas.Length - 1; i++)
                            s += komandasDaļas[i] + " ";
                        s = s.Trim();
                        rezultāts += _endl + komandasDaļas[1] + " db \"" + s + "\",0";

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
                        rezultāts = Calculate(rezultāts, komandasDaļas, metode);
                    }
                    else if (komandasDaļas[2] == "-")
                    {
                        metode = "sub";
                        rezultāts = Calculate(rezultāts, komandasDaļas, metode);
                    }

                }
                if (Nosaukums == "VAI")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }

                    if (komandasDaļas[2] == "!")
                    {
                        rezultāts += "\r\nmov eax, " + komandasDaļas[1] + Environment.NewLine + "cmp eax, " + komandasDaļas[3];
                        rezultāts += Environment.NewLine + "je leciens" + _ifJumpNumber;
                    }
                    else if (komandasDaļas[2] == "=")
                    {
                        rezultāts += "\r\nmov eax, " + komandasDaļas[1] + Environment.NewLine + "cmp eax, " + komandasDaļas[3];
                        rezultāts += Environment.NewLine + "jne leciens" + _ifJumpNumber;
                    }
                }
                if (Nosaukums == "NĒ")
                {
                    rezultāts += "\r\njmp parleciens" + _ifJumpNumber;
                    rezultāts += "\r\nleciens" + _ifJumpNumber + ":";
                }
                if (Nosaukums == "PĀRSTĀT")
                {
                    rezultāts += "\r\nparleciens" + _ifJumpNumber + ":";
                    _ifJumpNumber++;
                }
                if (Nosaukums == "ATKĀRTOT")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += "\r\natkartojums" + _whileJumpNumber + ":";
                }
                if (Nosaukums == "KAMĒR")
                {
                    rezultāts += Environment.NewLine + "mov ecx, " + komandasDaļas[1];
                    rezultāts += Environment.NewLine + "cmp ecx, " + komandasDaļas[3];
                    rezultāts += Environment.NewLine + "je atkartojums" + _whileJumpNumber;
                    _whileJumpNumber++;
                }
                if (Nosaukums == "KAMĒRNAV")
                {
                    rezultāts += Environment.NewLine + "mov ecx, " + komandasDaļas[1];
                    rezultāts += Environment.NewLine + "cmp ecx, " + komandasDaļas[3];
                    rezultāts += Environment.NewLine + "jne atkartojums" + _whileJumpNumber;
                    _whileJumpNumber++;
                }
                if (Nosaukums == "IZVADĪT")
                {
                    rezultāts += AddCodeHeader(ref VaiIrKods);

                    rezultāts += Print(komandasDaļas[1]);
                }
                if (Nosaukums == "PIEŠĶIRT")
                {
                    rezultāts += AddCodeHeader(ref VaiIrKods);

                    rezultāts += Assign(komandasDaļas[2], komandasDaļas[1]);
                }
                if (Nosaukums == "RINDA")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    string kompilacijasRezultāts = CallBuiltInCompiler("rnda");
                    if (kompilacijasRezultāts == "err rnda")
                    {
                        MessageBox.Show("Kļūda kompilātorā err rnda");
                        return "";
                    }
                    rezultāts += Environment.NewLine + kompilacijasRezultāts;
                }
                if (Nosaukums == "PALIELINĀT")
                {
                    rezultāts += Environment.NewLine + "mov eax , " + komandasDaļas[1] + Environment.NewLine + "inc eax" + _endl + "mov " + komandasDaļas[1] + ", eax";
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
                    rezultāts += _endl + "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "and ecx, 00000000000000000000000011111111b" + Environment.NewLine +
                                 "mov " + komandasDaļas[2] + ", ecx" + Environment.NewLine +
                                 "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "shr ecx,8" + Environment.NewLine +
                                 "and ecx, 00000000000000000000000011111111b" + _endl +
                                 "mov " + komandasDaļas[3] + ", ecx" + Environment.NewLine +
                                 "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "shr ecx,16" + Environment.NewLine +
                                 "and ecx, 00000000000000000000000011111111b" + _endl +
                                 "mov " + komandasDaļas[4] + ", ecx" + Environment.NewLine +
                                 "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "shr ecx,24" + Environment.NewLine +
                                 "and ecx, 00000000000000000000000011111111b" + _endl +
                                 "mov " + komandasDaļas[5] + ", ecx" + Environment.NewLine;
                }
                if (Nosaukums == "IZVADĪTČARU")
                {
                    rezultāts += _endl + "mov ecx," + komandasDaļas[1] + _endl + "mov x, cl" + _endl + "invoke StdOut, addr x";
                }
                if (Nosaukums == "IZVADĪTTEKSTU")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += _endl + "invoke StdOut, addr " + komandasDaļas[1];
                }
                if (Nosaukums == "IELASĪTČARU")
                {
                    if (VaiIrKods == false)
                    {
                        VaiIrKods = true; rezultāts = rezultāts + "\r\n" + @" .code
main PROC";
                    }
                    rezultāts += Environment.NewLine + "mov eax , input()" + Environment.NewLine + "mov " + komandasDaļas[1] + " , eax" + _endl
                        + "mov eax, " + komandasDaļas[1] + Environment.NewLine +
                                 "mov ecx , [eax] " + Environment.NewLine +
                                 "and ecx, 00000000000000000000000011111111b" + Environment.NewLine +
                                 "mov " + komandasDaļas[1] + ", ecx";

                }

            }
            if (rezultāts != "")
            {
                rezultāts = rezultāts + @"
ret
 main ENDP
END main";
            }

            return rezultāts;
        }

        private static string Assign(string assignFrom, string assignTo)
        {
            return Environment.NewLine + "mov eax, " + assignFrom + Environment.NewLine + "mov " + assignTo + " , eax";
        }

        string CallBuiltInCompiler(string p)
        {

            string nos = null;
            string[] param = p.Split(' ');
            StreamReader outputReader = null;
            StreamReader errorReader = null;
            switch (param[0])
            {
                case "rnda":
                    nos = "scsr";
                    break;
                case "nskt":
                    nos = "scsn";
                    break;
            }

            try
            {


                //Create Process Start information
                ProcessStartInfo processStartInfo =
                    new ProcessStartInfo(@"C:\\temp\\" + nos + ".exe");
                processStartInfo.ErrorDialog = false;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;

                //Execute the process
                Process process = new Process();
                process.StartInfo = processStartInfo;
                bool processStarted = process.Start();
                if (processStarted)
                {
                    //Get the output stream
                    StreamWriter myStreamWriter = process.StandardInput;

                    myStreamWriter.WriteLine(param[0]);
                    if (param.Length == 2)
                        myStreamWriter.WriteLine(param[1]);


                    myStreamWriter.Close();

                    outputReader = process.StandardOutput;
                    errorReader = process.StandardError;
                    process.WaitForExit();

                    return outputReader.ReadToEnd();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (outputReader != null)
                {
                    outputReader.Close();
                }
                if (errorReader != null)
                {
                    errorReader.Close();
                }
            }

            return null;

        }

        #region "Palīgmetodes"

        bool IsNumber(string s)
        {
            int rezultats;
            try
            {
                rezultats = int.Parse(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        string AddCodeHeader(ref bool isCode) 
        {
            if (isCode == true)
                return "";
            else
            {
                isCode = true;
                return "\r\n" + @" .code" + _endl + "main PROC";
            }
        }

        #endregion

        #endregion
    }
}