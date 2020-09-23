using System;
using System.IO;
using System.Text.Json;

namespace PasswordChecking
{
    public delegate void Message(string message);

    public class PasswordRequirements
    {
        public string Symbols1 { set; get; }
        public string Symbols2 { set; get; }
        public string Symbols3 { set; get; }
        public string Symbols4 { set; get; }
        public int PasswordLength { set; get; }
    }
    public class PasswordCheck
    {
        public event Message Error;
        public event Message Success;

        private readonly string symbols1;
        private readonly string symbols2;
        private readonly string symbols3;
        private readonly string symbols4;

        private readonly string symbols;
        private readonly int passwordLength;

        public PasswordCheck()
        {
            /*
            symbols1 = "QWERTYUIOPASDFGHJKLZXCVBNM";
            symbols2 = "qwertyuiopasdfghjklzxcvbnm";
            symbols3 = "!@#$%^&*:;.,-_";
            symbols4 = "0123456789";
            
            passwordLength = 8;
            */

            PasswordRequirements passwordRequirements;
            using (var file = new FileStream("password.json", FileMode.Open))
            {
                passwordRequirements = JsonSerializer.DeserializeAsync<PasswordRequirements>(file).Result;
            }

            symbols1 = passwordRequirements.Symbols1;
            symbols2 = passwordRequirements.Symbols2;
            symbols3 = passwordRequirements.Symbols3;
            symbols4 = passwordRequirements.Symbols4;

            symbols = symbols1 + symbols2 + symbols3 + symbols4;

            passwordLength = passwordRequirements.PasswordLength;

        }

        public PasswordCheck(int length, string symbols1, string symbols2, string symbols3, string symbols4)
        {
            passwordLength = length;
            this.symbols1 = symbols1;
            this.symbols2 = symbols2;
            this.symbols3 = symbols3;
            this.symbols4 = symbols4;

            symbols = this.symbols1 + this.symbols2 + this.symbols3 + this.symbols4;
        }

        public bool CheckLength(string password)
        {
            password = password.Trim();

            if (password.Length < passwordLength)
            {
                Error?.Invoke($"Длина пароля меньше {passwordLength} символов");
                return false;
            }
            else
            {
                Success?.Invoke($"Длина пароля соответствует минимальной длинне");
                return true;
            }
        }

        public void ClearEvents()
        {
            Success = null;
            Error = null;
        }

        public bool CheckSymbol(string password)
        {
            var pass_chek1 = password.IndexOfAny(symbols1.ToCharArray());
            var pass_chek2 = password.IndexOfAny(symbols2.ToCharArray());
            var pass_chek3 = password.IndexOfAny(symbols3.ToCharArray());
            var pass_chek4 = password.IndexOfAny(symbols4.ToCharArray());

            if (pass_chek1 == -1 || pass_chek2 == -1 || pass_chek3 == -1 || pass_chek4 == -1)
            {
                Error?.Invoke("Пароль не соответствует требованиям безопасности");
                return false;
            }
            else
            {
                Success?.Invoke("Пароль соответствует минимальным требованиям");
                return true;
            }
        }

        public bool CheckAlphabet(string password)
        {
            foreach (var symbol in password)
            {
                if (!symbols.Contains(symbol))
                {
                    Error?.Invoke("Пароль содержит запрещённые символы");
                    return false;
                }
            }
            Success?.Invoke("Пароль не содержит запрещённые символы");
            return true;
        }
    }
}
