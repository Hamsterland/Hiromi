namespace Hiromi.Services.Text
{
    public static class TextUtilities
    {
        public static string Uwufy(string text)
        {
            var result = string.Empty;
            
            for (var i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case 'l':
                    case 'r':
                    {
                        result += "w";
                        break;
                    }
                    case 'm':
                    case 'n':
                    {
                        if (text[i + 1] == 'o')
                        {
                            result += text[i];
                            result += 'y';
                        }
                        else
                        {
                            result += text[i];
                        }
			
                        break;
                    }
                    default:
                    {
                        result += text[i];
                        break;
                    }
                }
            }

            return result;
        }
    }
}