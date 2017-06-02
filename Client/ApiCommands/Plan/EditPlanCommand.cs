using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class EditPlanCommand : InputOutputCommand<PlanClientData, PlanView>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Edit plan"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/plan"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            var plan = new PlanClientData();
            plan.Windows = new List<WindowClientData>()
            {
                new WindowClientData() { WindowID = -10, BarLocation = BarLocation.Top },
            };
            plan.MainContainer = new MainContainerClientData()
            {
                Floors = new List<FloorClientData>()
                {
                    new FloorClientData()
                    {
                        ContainerID = -1,
                        Name = "Floor 1",
                        SequenceNumber = 1,
                        Rooms = new List<RoomClientData>()
                        {
                            new RoomClientData()
                            {
                                ContainerID = -2,
                                Name = "Room 1",
                                SequenceNumber = 1,
                                Settings = new RoomSettings()
                                {
                                    IconId = 1,
                                },
                                Walls = new List<WallClientData>()
                                {
                                    new WallClientData()
                                    {
                                        ContainerID = -3,
                                        Name = "Wall 1",
                                        SequenceNumber = 1,
                                        Settings = new WallSettings()
                                        {
                                            IsSkylight = false,
                                            Width = 6,
                                            Height = 3,
                                        },
                                        WindowIDs = new List<int>() { plan.Windows.First().WindowID, },
                                    },
                                },
                                Groups = new List<GroupClientData>()
                                {
                                    new GroupClientData()
                                    {
                                        ContainerID = -4,
                                        Name = "Group 1",
                                        Settings = new GroupSettings()
                                        {
                                            Color = 0xEEEEEE,
                                        },
                                        WindowIDs = new List<int>() { plan.Windows.First().WindowID, },
                                    },
                                },
                            }
                        },
                    },
                },
            };

            return plan;
        }

        protected override object GenerateBodyRequest()
        {
            var plan = new PlanClientData();
            plan.Windows = GenerateWindows();
            var minContainerID = plan.Windows.Min(x => x.WindowID) - 1;
            plan.MainContainer = GenerateMainContainer(ref minContainerID);

            var walls = plan.MainContainer.Floors.SelectMany(f => f.Rooms.SelectMany(r => r.Walls)).ToList();
            var windowsIDsStack = new Stack<int>(plan.Windows.Select(x => x.WindowID));
            while (windowsIDsStack.Count > 0)
            {
                foreach (var wall in walls)
                {
                    if (StaticRandom.Next(3) != 0)
                        continue;

                    if (wall.WindowIDs == null)
                        wall.WindowIDs = new List<int>();

                    wall.WindowIDs.Add(windowsIDsStack.Pop());
                    if (windowsIDsStack.Count == 0)
                        break;
                }
            }

            var rooms = plan.MainContainer.Floors.SelectMany(f => f.Rooms).ToList();
            foreach (var room in rooms)
            {
                var roomWindowIDs = room.Walls.Where(w => w.WindowIDs != null).SelectMany(w => w.WindowIDs);
                foreach (var group in room.Groups)
                {
                    foreach (var windowID in roomWindowIDs)
                    {
                        if (StaticRandom.Next(3) != 0)
                            continue;

                        if (group.WindowIDs == null)
                            group.WindowIDs = new List<int>();

                        group.WindowIDs.Add(windowID);
                    }
                }
            }

            return plan;
        }

        List<WindowClientData> GenerateWindows()
        {
            var windows = new List<WindowClientData>();
            for (int i = 1; i < StaticRandom.Next(10) + 10; i++)
            {
                var window = new WindowClientData()
                {
                    WindowID = -i,
                    SequenceNumber = i,
                    X = (StaticRandom.Next(150) + 50) / 10f,
                    Y = (StaticRandom.Next(150) + 50) / 10f,
                    Width = (StaticRandom.Next(150) + 50) / 10f,
                    Height = (StaticRandom.Next(250) + 50) / 10f,
                    BarLocation = BarLocation.Top,
                };
                window.Description = $"Window about {window.Width}m. width and {window.Height}m. height";
                window.DiagonalLTRB = (float)Math.Sqrt(window.Width * window.Width + window.Height * window.Height);
                window.DiagonalRTLB = window.DiagonalLTRB;

                windows.Add(window);
            }
            return windows;
        }

        MainContainerClientData GenerateMainContainer(ref int nextContainerID)
        {
            var mainContainer = new MainContainerClientData()
            {
                ContainerID = 0,
                Floors = GenerateFloors(ref nextContainerID),
            };

            return mainContainer;
        }

        List<FloorClientData> GenerateFloors(ref int nextContainerID)
        {
            var floors = new List<FloorClientData>();
            for (int i = 1; i < StaticRandom.Next(2) + 2; i++)
            {
                var floor = new FloorClientData()
                {
                    ContainerID = nextContainerID--,
                    Name = $"Floor {i}",
                    SequenceNumber = i,
                    Rooms = GenerateRooms(ref nextContainerID),
                };

                floors.Add(floor);
            }
            return floors;
        }

        List<RoomClientData> GenerateRooms(ref int nextContainerID)
        {
            var rooms = new List<RoomClientData>();
            for (int i = 1; i < StaticRandom.Next(5) + 2; i++)
            {
                var room = new RoomClientData()
                {
                    ContainerID = nextContainerID--,
                    Name = $"Room {i}",
                    SequenceNumber = i,
                    Settings = new RoomSettings() { IconId = StaticRandom.Next(5) },
                };
                room.Walls = GenerateWalls(ref nextContainerID);
                room.Groups = GenerateGroups(ref nextContainerID);

                rooms.Add(room);
            }
            return rooms;
        }

        List<WallClientData> GenerateWalls(ref int nextContainerID)
        {
            var walls = new List<WallClientData>();
            for (int i = 1; i < StaticRandom.Next(5) + 2; i++)
            {
                var wall = new WallClientData()
                {
                    ContainerID = nextContainerID--,
                    Name = $"Wall {i}",
                    SequenceNumber = i,
                    Settings = new WallSettings()
                    {
                        IsSkylight = false,
                        Width = StaticRandom.Next(3) + 3,
                        Height = StaticRandom.Next(2) + 2,
                    },
                };

                walls.Add(wall);
            }

            return walls;
        }

        List<GroupClientData> GenerateGroups(ref int nextContainerID)
        {
            var groups = new List<GroupClientData>();
            for (int i = 1; i < StaticRandom.Next(2) + 2; i++)
            {
                var group = new GroupClientData()
                {
                    ContainerID = nextContainerID--,
                    Name = $"Group {i}",
                    Settings = new GroupSettings() { Color = StaticRandom.Next(Int32.MaxValue) },
                };

                groups.Add(group);
            }

            return groups;
        }

        public async Task<CommandResponse<PlanView>> ExecuteAsync(CloudClient client, long installationID, PlanClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
