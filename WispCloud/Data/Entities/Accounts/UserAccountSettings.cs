namespace DeusCloud.Data.Entities.Accounts
{
    public sealed class UserSettings
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }

}