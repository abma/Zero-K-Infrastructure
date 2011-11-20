using System.Linq;
using LobbyClient;
using ZkData;

namespace CaTracker
{
    public class PlayerMover
    {
        readonly TasClient tas;

        public PlayerMover(TasClient tas)
        {
            this.tas = tas;
            tas.Said += tas_Said;
        }

        void tas_Said(object sender, TasSayEventArgs e)
        {
            if (e.Place == TasSayEventArgs.Places.Normal && e.Text.StartsWith("!move"))
            {
                var db = new ZkDataContext();
                var acc = db.Accounts.Single(x => x.Name == e.UserName);
                if (acc.IsZeroKAdmin || acc.IsLobbyAdministrator)
                {
                    var parts = e.Text.Split(' ');
                    if (parts.Length != 3) tas.Say(TasClient.SayPlace.User, e.UserName, "!move [from] [to]", false);
                    else
                    {
                        var from = tas.ExistingBattles.Values.FirstOrDefault(x => x.Founder.Name == parts[1]);
                        var to = tas.ExistingBattles.Values.FirstOrDefault(x => x.Founder.Name == parts[2]);
                        if (from != null && to != null)
                        {
                            foreach (var b in from.Users) if (!b.LobbyUser.IsInGame) tas.Say(TasClient.SayPlace.User, b.Name, "!join " + parts[2], false);
                        }
                        else tas.Say(TasClient.SayPlace.User, e.UserName, "Not a valid battle host name", false);
                    }
                }
            }
        }
    }
}