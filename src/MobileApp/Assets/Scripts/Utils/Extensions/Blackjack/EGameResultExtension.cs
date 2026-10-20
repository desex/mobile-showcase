using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.Utils.Extensions.Blackjack
{
    public static class EGameResultExtension
    {
        public static string ToText(this EGameResult gameResult, int count)
        {
            var resultText = count.ToString();
            switch (gameResult)
            {
                case EGameResult.Win:
                    switch (count)
                    {
                        case 0:
                            return resultText + " побед";
                        case 1:
                            return resultText + " победа";
                        case 2:
                        case 3:
                            return resultText + " победы";
                    }

                    break;
                
                case EGameResult.Lost:
                    switch (count)
                    {
                        case 0:
                            return resultText + " поражений";
                        case 1:
                            return resultText + " поражение";
                        case 2:
                        case 3:
                            return resultText + " поражения";
                    }
                    break;
            }

            return resultText;
        }
    }
}