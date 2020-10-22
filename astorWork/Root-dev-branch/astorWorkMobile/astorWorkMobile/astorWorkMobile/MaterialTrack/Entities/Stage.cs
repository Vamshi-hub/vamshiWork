namespace astorWorkMobile.MaterialTrack.Entities
{
    public class Stage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        private string _color;
        public string Colour
        {
            get
            {
                return _color;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 9)
                {
                    _color = "#" + value.Substring(7) + value.Substring(1, 6);
                }
                else
                {
                    _color = value;
                }
            }
        }

        public int MilestoneId { get; set; }
        public bool IsEditable { get; set; }
        public string MaterialTypes { get; set; }
    }
}
