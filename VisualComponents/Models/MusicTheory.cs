using NotionPlay.VisualComponents.Enums;

namespace NotionPlay.VisualComponents.Models
{
    /// <summary>
    /// ✨ 乐理结构,维系音乐播放时间精度的必要项
    /// </summary>
    public sealed class MusicTheory()
    {
        private int _basicvalue = 3000;
        public int BasicValue
        {
            get => _basicvalue;
            set => _basicvalue = value > 0 ? value : _basicvalue;
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

        public int GetSpan(DurationTypes durationType) => BasicValue / Math.Clamp((int)durationType, 1, 64);

        private void ReCalculateBasicValue() => BasicValue = (int)(60000f / Speed * LeftNum);
    }
}
