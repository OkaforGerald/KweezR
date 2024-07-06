using Entities.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SharedAPI.TransferObjects;

namespace KweezR.Client.Pages
{
    public partial class Rooms
    {
        private HubConnection? hubConnection;
        private List<RoomsDto>? rooms;
		private bool showModal = false;
		private CreateRoomDto newRoom = new CreateRoomDto();

		protected override async Task OnInitializedAsync()
        {
            hubConnection = await RoomService.ConfigureHubConnection();

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

			hubConnection.On<Guid>("SendId", (id) =>
			{
				JoinRoom(id);
			});

			await hubConnection.StartAsync();
        }

        public void JoinRoom(Guid Id)
        {
            nav.NavigateTo($"game/{Id}");
        }

		private void OpenModal()
		{
			showModal = true;
		}

		private void CloseModal()
		{
			showModal = false;
			newRoom = new CreateRoomDto();
		}

		private async Task CreateRoom()
		{
			await hubConnection!.SendAsync("CreateRoom", newRoom);
			CloseModal();
		}
	}
}
