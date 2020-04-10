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
                Worlds.Add(world.Id, new World()
                {
                    Id = world.Id, Name = world.Name, Population = world.Population
                });
            }

            foreach (var match in matchesResult)
            {
                CreateTeamInfo(match, "Green", Color.DarkGreen);
                CreateTeamInfo(match, "Blue", Color.DarkBlue);
                CreateTeamInfo(match, "Red", Color.DarkRed);
            }
        }

        private static void CreateTeamInfo(Match match, string teamColor, Color displayColor)
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
                Score = Util.GetPropertyValue<int>(match.Skirmishes.LastOrDefault()?.Scores, teamColor),
                VP_Tip = "Highest\t " + hvp.ToString() + "\r\n" + "Lowest\t " + lvp.ToString(),
                Link_Tip = GenerateMatchInfoTip(Util.GetPropertyValue<IEnumerable<int>>(match.All_Worlds, teamColor)),
                Placeholder = Worlds[Util.GetPropertyValue<int>(match.Worlds, teamColor)].Name,
                TextColor = Color.Gainsboro
            });
        }

        private void radioButtonNA_CheckedChanged(object sender, System.EventArgs e)
        {
            // This event is fired both when the radio is checked and when it is unchecked.
            // We only care about when it's been checked.
            if (!radioButtonNA.Checked)
            {
                return;
            }

            // start at the end and loop backwards to prevent issues with indexes resetting when disposing
            for (var i = resultsPanel.Controls.Count - 1; i >= 0; i--)
            {
                resultsPanel.Controls[i].Dispose();
            }

            DisplayInfo("NA");
        }

        private void radioButtonEU_CheckedChanged(object sender, System.EventArgs e)
        {
            // This event is fired both when the radio is checked and when it is unchecked.
            // We only care about when it's been checked.
            if (!radioButtonEU.Checked)
            {
                return;
            }

            // start at the end and loop backwards to prevent issues with indexes resetting when disposing
            for (var i = resultsPanel.Controls.Count - 1; i >= 0; i--)
            {
                resultsPanel.Controls[i].Dispose();
            }

            DisplayInfo("EU");
        }

        private void DisplayInfo(string selectedRegion)
        {
            var displayTeams = Teams.Where(team => team.Region == selectedRegion).ToList();
            var sortedList =
                displayTeams
                    .OrderBy(a => a.Tier)
                    .ThenByDescending(a => a.VP)
                    .ThenBy(a => a.Score)
                    .ToList();

            var numTiers = sortedList.Max(t => t.Tier);

            for (var tier = 1; tier <= numTiers; tier++)
            {
                #region Configurable values to alter the appearance of the table

                const int spacing = 5;
                const int rowHeight = 23;
                const int serverColumnWidth = 160;
                const int lockColumnWidth = 23;
                const int rankColumnWidth = 50;
                const int vpColumnWidth = 100;
                const int scoreColumnWidth = 100;
                const int nextMatchColumnWidth = 160;

                #endregion

                var spacingObj = new Padding(0, 0, spacing, spacing);

                var tierTable = new TableLayoutPanel()
                {
                    Padding = new Padding(0, 10, 0, 0), // add some spacing between each tier
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Font = new Font("Cambria", 11, FontStyle.Regular),
                    ForeColor = Color.FromArgb(255, 128, 128, 128),
                    BackColor = Color.Black
                };

                tierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, serverColumnWidth + spacing));
                tierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, lockColumnWidth + spacing));
                tierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, rankColumnWidth + spacing));
                tierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, vpColumnWidth + spacing));
                tierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, scoreColumnWidth + spacing));
                tierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, nextMatchColumnWidth + spacing));
                tierTable.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight + spacing));
                tierTable.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight + spacing));
                tierTable.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight + spacing));
                tierTable.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight + spacing));

                #region Column Headers

                var headerControls = new List<Control>
                {
                    new Label // ---------------------------------- Current Team Name
                    {
                        Dock = DockStyle.Fill,
                        Margin = spacingObj,
                        TextAlign = ContentAlignment.TopCenter,
                        Text = $@"Current Tier {tier} Matchup"
                    },
                    new Label // ----------------------------- Lock Image (no heading, just here as a spacer)
                    {
                        Margin = spacingObj,
                    },
                    new Label // ---------------------------------- Rank
                    {
                        Dock = DockStyle.Fill,
                        Margin = spacingObj,
                        TextAlign = ContentAlignment.TopCenter,
                        Text = @"Rank"
                    },
                    new Label // ---------------------------------- Victory Points
                    {
                        Dock = DockStyle.Fill,
                        Margin = spacingObj,
                        TextAlign = ContentAlignment.TopCenter,
                        Text = @"Victory Points"
                    },
                    new Label // ---------------------------------- War Score
                    {
                        Dock = DockStyle.Fill,
                        Margin = spacingObj,
                        TextAlign = ContentAlignment.TopCenter,
                        Text = @"War Score"
                    },
                    new Label // ---------------------------------- Next Team Name
                    {
                        Dock = DockStyle.Fill,
                        Margin = spacingObj,
                        TextAlign = ContentAlignment.TopCenter,
                        Text = $@"Next Tier {tier} Matchup"
                    }
                };
                for (var i = 0; i < headerControls.Count; i++)
                {
                    tierTable.Controls.Add(headerControls[i], i, 0);
                }

                #endregion


                #region Team rows

                const int numTeams = 3;

                for (var team = 1; team <= numTeams; team++)
                {
                    // calculate index of this team in the sortedList of all teams
                    var idx = (tier - 1) * numTeams + (team - 1);

                    if (team < numTeams && sortedList[idx].VP == sortedList[idx + 1].VP)
                    {
                        sortedList[idx].TextColor = Color.Salmon;
                        sortedList[idx + 1].TextColor = Color.Salmon;
                    }

                    if (team == numTeams && tier < numTiers)
                    {
                        var swap = sortedList[idx - 1].VP != sortedList[idx].VP;

                        if (sortedList[idx + 1].VP == sortedList[idx + 2].VP)
                        {
                            swap = false;
                        }

                        if (swap)
                        {
                            var temp = sortedList[idx].Placeholder;
                            sortedList[idx].Placeholder = sortedList[idx + 1].Placeholder;
                            sortedList[idx + 1].Placeholder = temp;
                        }
                    }

                    var teamControls = new List<Control>
                    {
                        new Label // ---------------------------------- Current Team Name
                        {
                            Dock = DockStyle.Fill,
                            Margin = spacingObj,
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = Color.Gainsboro,
                            BackColor = sortedList[idx].Color,
                            Text = sortedList[idx].Name
                        },
                        new PictureBox // ----------------------------- Lock Image
                        {
                            Dock = DockStyle.Fill,
                            Margin = spacingObj,
                            BackColor = Color.FromArgb(255, 28, 28, 28),
                        },
                        new Label // ---------------------------------- Rank
                        {
                            Dock = DockStyle.Fill,
                            Margin = spacingObj,
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = Color.Gainsboro,
                            Text = team == 1 ? "1st" : team == 2 ? "2nd" : team == 3 ? "3rd" : ""
                        },
                        new Label // ---------------------------------- Victory Points
                        {
                            Dock = DockStyle.Fill,
                            Margin = spacingObj,
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = sortedList[idx].TextColor,
                            Text = sortedList[idx].VP.ToString()
                        },
                        new Label // ---------------------------------- War Score
                        {
                            Dock = DockStyle.Fill,
                            Margin = spacingObj,
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = Color.Gainsboro,
                            Text = sortedList[idx].Score.ToString()
                        },
                        new Label // ---------------------------------- Next Team Name
                        {
                            Dock = DockStyle.Fill,
                            Margin = new Padding(0, 0, 0, spacing),
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = Color.Gainsboro,
                            BackColor = team == 1 ? Color.DarkGreen :
                                team == 2 ? Color.DarkBlue :
                                team == 3 ? Color.DarkRed : Color.Black,
                            Text = sortedList[idx].Placeholder
                        }
                    };
                    for (var i = 0; i < teamControls.Count; i++)
                    {
                        tierTable.Controls.Add(teamControls[i], i, team);
                    }
                }

                #endregion

                resultsPanel.Controls.Add(tierTable);
            }

            // dynamically resize the window:
            Size = new Size(
                resultsPanel.Width + resultsPanel.Padding.Horizontal,
                resultsPanel.Top + resultsPanel.Height + resultsPanel.Padding.Vertical + 20
            );
        }
    }
}