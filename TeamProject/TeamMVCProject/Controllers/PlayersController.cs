using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeamMVCProject.Context;
using TeamMVCProject.Models;
using TeamMVCProject.Repository;
using TeamMVCProject.ViewModels;

namespace TeamMVCProject.Controllers
{
   
    public class PlayersController : Controller
    {

        private PlayerRepository playerRepository = new PlayerRepository();

        // TODO: repository pattern for player
        // TODO HTTP Post/GeT   + Routing 
        // TODO: ApiController + AJAX

        // GET: Players
        public async Task<ActionResult> Index()
        {
            return View(await playerRepository.IncludePlayerTeams());
        }

        // GET: Players/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await playerRepository.FindAsync(Convert.ToInt32(id));
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.PlayerID = new SelectList(await playerRepository.GetPlayers(), "ID", "PlayerName");
            var player = new Player
            {
                Teams = new List<Team>()
            };
            ViewBag.Teams = await PopulateAssignedTeamData(player);
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Description")] Player player, int[] selectedTeams) // TODO view model
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Teams = await PopulateAssignedTeamData(player);
                return View(player);
            }

            if (selectedTeams != null)
            {
                await playerRepository.SetPlayerTeams(player, selectedTeams);
            }

            playerRepository.AddPlayer(player);
            return RedirectToAction("Index");
        }

        // GET: Players/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var players = await playerRepository.IncludePlayerTeams();
            var playerToPopulate = players.Where(i => i.ID == id).Single();//playerRepository.FindAsync(Convert.ToInt32(id));
            if (playerToPopulate == null)
            {
                return HttpNotFound();
            }

            ViewBag.Teams = await PopulateAssignedTeamData(playerToPopulate);
            return View(playerToPopulate);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, string[] selectedTeams)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var players = await playerRepository.IncludePlayerTeams();
            var playerToUpdate = players.Where(i => i.ID == id).Single();

            if (ModelState.IsValid)
            {
                await UpdatePlayerTeams(selectedTeams, playerToUpdate);
                playerRepository.Edit(playerToUpdate);

                return RedirectToAction("Index");
            }

            ViewBag.Teams = await PopulateAssignedTeamData(playerToUpdate);
           
            return View(playerToUpdate);
        }

        // GET: Players/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await playerRepository.FindAsync(Convert.ToInt32(id));
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await playerRepository.Remove(id);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Populates player with teams.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private async Task<List<PlayerTeamVM>> PopulateAssignedTeamData(Player player)
        {
            var allTeams = await playerRepository.GetTeams();
            var playerTeams = new HashSet<int>(player.Teams.Select(t => t.ID));
            var viewModel = new List<PlayerTeamVM>();
            foreach (var team in allTeams)
            {
                viewModel.Add(new PlayerTeamVM
                {
                    ID = team.ID,
                    Name = team.Name,
                    isAssigned = playerTeams.Contains(team.ID)
                });
            }
            return viewModel;            
        }

        private async Task UpdatePlayerTeams(string[] selectedTeams, Player playerToUpdate)
        {
            if (selectedTeams == null)
            {
                playerToUpdate.Teams = new List<Team>();
                return;
            }

            var allTeams = await playerRepository.GetTeams();
            var selectedTeamsHS = new HashSet<string>(selectedTeams);
            var playerTeams = new HashSet<int>
                (playerToUpdate.Teams.Select(t => t.ID));
            foreach (var team in allTeams)
            {
                if (selectedTeamsHS.Contains(team.ID.ToString()))
                {
                    if (!playerTeams.Contains(team.ID))
                    {
                        playerToUpdate.Teams.Add(team);
                    }
                }
                else
                {
                    if (playerTeams.Contains(team.ID))
                    {
                        playerToUpdate.Teams.Remove(team);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
