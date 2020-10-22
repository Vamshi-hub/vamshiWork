namespace MaterialTrackApp.DB
{
    public class BuildingEntity: MasterEntity
    {
        public string Project { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
    }
}
