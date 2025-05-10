using NotionPlay.VisualComponents.Enums;

namespace NotionPlay.VisualComponents
{
    public sealed class MusicTheory()
    {
        private int _basicvalue = 3000;
        public int BasicValue
        {
            get => _basicvalue;
            set => _basicvalue = value > 0 ? value : 3000;
        }

        private int _speed = 80;
        public int Speed
        {
            get => _speed;
            set
            {
                _speed = value > 0 ? value : 80;
                ReCalculateBasicValue();
            }
        }

        private int _leftnum = 4;
        public int LeftNum
        {
            get => _leftnum;
            set => _leftnum = value > 0 ? value : 4;
        }

        private int _rightnum = 4;
        public int RightNum
        {
            get => _rightnum;
            set
            {
                _rightnum = value > 0 ? value : 4;
                ReCalculateBasicValue();
            }
        }

        public int GetSpan(DurationTypes durationType)
        {
            return durationType switch
            {
                DurationTypes.Sixteen => BasicValue / 16,
                DurationTypes.Eight => BasicValue / 8,
                DurationTypes.Four => BasicValue / 4,
                DurationTypes.Two => BasicValue / 2,
                DurationTypes.One => BasicValue,
                _ => 0
            };
        }

        private void ReCalculateBasicValue() => BasicValue = (int)((60000f / Speed) * LeftNum);
    }
}
