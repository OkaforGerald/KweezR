using Microsoft.AspNetCore.SignalR.Client;
using SharedAPI.TransferObjects;

namespace KweezR.Client.Pages
{
    public partial class Rooms
    {
        private HubConnection? hubConnection;
        private List<RoomsDto>? rooms;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = RoomService.ConfigureHubConnection();

            hubConnection.On<List<RoomsDto>>("SendRooms", (rooms) =>
            {
                this.rooms = rooms;
                StateHasChanged();
            });

			hubConnection.On<List<RoomsDto>>("SendUpdate", (rooms) =>
			{
				this.rooms = rooms;
				StateHasChanged();
			});

			await hubConnection.StartAsync();
        }

        public void JoinRoom(Guid Id)
        {
            nav.NavigateTo($"game/{Id}");
        }
    }
}
