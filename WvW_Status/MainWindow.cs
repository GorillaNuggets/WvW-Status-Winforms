using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using WvW_Status.Utilities;

namespace WvW_Status
{
    public partial class MainWindow : Form
    {
        private static readonly List<Team> Teams = new List<Team>();
        private static readonly Dictionary<int, World> Worlds = new Dictionary<int, World>();

        public MainWindow()
        {
            InitializeComponent();

            var client = new WebClient();
            var worldsData = client.DownloadString("https://api.guildwars2.com/v2/worlds?ids=all");
            var worldResult = JsonConvert.DeserializeObject<List<World>>(worldsData);

            var matchesData = client.DownloadString("https://api.guildwars2.com/v2/wvw/matches?ids=all");
            var matchesResult = JsonConvert.DeserializeObject<List<Match>>(matchesData);
            client.Dispose();

            foreach (var world in worldResult)
            {
                Worlds.Add(world.Id, new World() { Id = world.Id, Name = world.Name, Population = world.Population });
            }

            foreach (var match in matchesResult)
            {
                CreateTeamInfo(match, "Green", Color.DarkGreen);
                CreateTeamInfo(match, "Blue", Color.DarkBlue);
                CreateTeamInfo(match, "Red", Color.DarkRed);
            }            
        }
        private void CreateTeamInfo(Match match, string teamColor, Color displayColor)
        {
            string GenerateMatchInfoTip(IEnumerable<int> list)
            {
                return list.Reverse().Aggregate(
                    "",
                    (current, id) => current + (Worlds[id].Name.PadRight(25) + "\t" + Worlds[id].Population + "\r\n")
                );
            }

            var vp = Util.GetPropertyValue<int>(match.Victory_Points, teamColor);
            var hvp = vp + ((85 - match.Skirmishes.Count) * 5);
            var lvp = vp + ((85 - match.Skirmishes.Count) * 3);

            Teams.Add(new Team()
            {
                Region = match.Id.Substring(0, 1) == "1" ? "NA" : "EU",
                Tier = int.Parse(match.Id.Substring(match.Id.Length - 1, 1)),
                Name = Worlds[Util.GetPropertyValue<int>(match.Worlds, teamColor)].Name,
                Color = displayColor,
                Image = null,
                VP = vp,
                HVP = hvp,
                LVP = lvp,
                Score = Util.GetPropertyValue<int>(match.Skirmishes.LastOrDefault().Scores, teamColor),
                VP_Tip = "Highest\t " + hvp.ToString() + "\r\n" + "Lowest\t " + lvp.ToString(),
                Link_Tip = GenerateMatchInfoTip(Util.GetPropertyValue<IEnumerable<int>>(match.All_Worlds, teamColor)),
                Placeholder = Worlds[Util.GetPropertyValue<int>(match.Worlds, teamColor)].Name
            });
        }
        private void radioButtonNA_CheckedChanged(object sender, System.EventArgs e)
        {            
            var displayTeams = Teams.Where(team => team.Region == "NA");
            DisplayInfo(displayTeams);
        }
        private void radioButtonEU_CheckedChanged(object sender, System.EventArgs e)
        {            
            var displayTeams = Teams.Where(team => team.Region == "EU");
            DisplayInfo(displayTeams);
        }
        private void DisplayInfo(IEnumerable<Team> displayTeams)
        {
            var sortedList = new List<Team>();
            sortedList = displayTeams.OrderBy(a => a.Tier).ThenByDescending(a => a.VP).ThenBy(a => a.Score).ToList();                       
            
            var tiers = sortedList.Max(t => t.Tier);
            this.Height = 105 + (tiers * 125);

            int pos = 0;
            int py = 61;
            for (int p = 1; p <= tiers; p++)
            {
                var matchPanel = new Panel
                {
                    Location = new Point(5, py),
                    Size = new Size(670, 120),
                    Font = new Font("Cambria", 11, FontStyle.Regular),                    
                    ForeColor = Color.FromArgb(255, 128, 128, 128),
                    BackColor = Color.Black,
                    Controls =
                    {
                        new Label
                        {
                            Location = new Point(8, 3),
                            AutoSize = true,                            
                            TextAlign = ContentAlignment.TopCenter,
                            Text = $"Current Tier {p} Matchup"
                        },
                        new Label
                        {
                            Location = new Point(198, 3),
                            AutoSize = true,
                            TextAlign = ContentAlignment.TopCenter,
                            Text = "Rank"
                        },
                        new Label
                        {
                            Location = new Point(263, 3),
                            AutoSize = true,
                            TextAlign = ContentAlignment.TopCenter,
                            Text = "Victory Points"
                        },
                        new Label
                        {
                            Location = new Point(386, 3),
                            AutoSize = true,
                            TextAlign = ContentAlignment.TopCenter,
                            Text = "War Score"
                        },
                        new Label
                        {
                            Location = new Point(518, 3),
                            AutoSize = true,
                            TextAlign = ContentAlignment.TopCenter,
                            Text = $"Next Tier {p} Matchup"
                        }
                    }
                };

                int ty = 28;
                var teamName = sortedList[pos].Name;
                
                for (int team = 1; team <= 3; team++)
                {
                    if (team == 3 && p < tiers) 
                    {
                        var swap = true;

                        if (sortedList[pos - 1].VP == sortedList[pos].VP)
                        {
                            swap = false;
                        }
                        if (sortedList[pos + 1].VP == sortedList[pos + 2].VP)
                        {
                            swap = false;        
                        }

                        if (swap == true)
                        {
                            var temp = sortedList[pos].Placeholder;
                            sortedList[pos].Placeholder = sortedList[pos + 1].Placeholder;
                            sortedList[pos + 1].Placeholder = temp;
                        }                        
                    }
                    var teamPanel = new Panel
                    {
                        Location = new Point(5, ty),
                        Size = new Size(670, 23),
                        Controls =
                        {
                            new Label // ---------------------------------- Current Team Name
                            {
                                Location = new Point(0, 0),
                                Size = new Size(160, 23),
                                TextAlign = ContentAlignment.MiddleCenter,
                                ForeColor = Color.Gainsboro,
                                BackColor = sortedList[pos].Color,
                                Text = sortedList[pos].Name
                            },
                            new PictureBox // ----------------------------- Lock Image
                            {
                                Location = new Point(165, 0),
                                Size = new Size(23, 23),
                                BackColor = Color.FromArgb(255, 28, 28, 28),
                            },
                            new Label // ---------------------------------- Rank
                            {
                                Location = new Point(193, 0),
                                Size = new Size(40, 23),
                                TextAlign = ContentAlignment.MiddleCenter,
                                ForeColor = Color.Gainsboro,
                                Text = team == 1 ? "1st" : team == 2 ? "2nd" : team == 3 ? "3rd" : ""
                            },
                            new Label // ---------------------------------- Victory Points
                            {
                                Location = new Point(258, 0),
                                Size = new Size(98, 23),
                                TextAlign = ContentAlignment.MiddleCenter,
                                ForeColor = Color.Gainsboro,
                                Text = sortedList[pos].VP.ToString()
                            },
                            new Label // ---------------------------------- War Score
                            {
                                Location = new Point(381, 0),
                                Size = new Size(72, 23),
                                TextAlign = ContentAlignment.MiddleCenter,
                                ForeColor = Color.Gainsboro,
                                Text = sortedList[pos].Score.ToString()
                            },
                            new Label // ---------------------------------- Next Team Name
                            {
                                Location = new Point(500, 0),
                                Size = new Size(160, 23),
                                TextAlign = ContentAlignment.MiddleCenter,
                                ForeColor = Color.Gainsboro,
                                BackColor = team == 1 ? Color.DarkGreen : team == 2 ? Color.DarkBlue : team == 3 ? Color.DarkRed : Color.Black,
                                Text = sortedList[pos].Placeholder
                            }
                        }
                    };
                    matchPanel.Controls.Add(teamPanel);
                    ty += 29;
                    pos++;
                }
                this.Controls.Add(matchPanel);
                py += 125;
            }
        }
    }
}