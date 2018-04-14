using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeamMVCProject.Context;
using TeamMVCProject.Models;
using TeamMVCProject.ViewModels;

namespace TeamMVCProject.Controllers
{
    public class PlayersController : Controller
    {
        private TeamsPlayersContext db = new TeamsPlayersContext();

        // GET: Players
        public ActionResult Index()
        {
            var players = db.Players.Include(p => p.Teams);
            return View(players.ToList());
        }

        // GET: Players/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            ViewBag.PlayerID = new SelectList(db.Players, "ID", "PlayerName");
            var player = new Player();
            player.Teams = new List<Team>();
            PopulateAssignedTeamData(player);
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description")] Player player, string[] selectedTeams)
        {
            if (selectedTeams != null)
            {
                player.Teams = new List<Team>();
                foreach (var team in selectedTeams)
                {
                    var teamToAdd = db.Teams.Find(int.Parse(team));
                    player.Teams.Add(teamToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.TeamID = new SelectList(db.Teams, "ID", "TeamName", player.TeamID);
            PopulateAssignedTeamData(player);
            return View(player);
        }

        // GET: Players/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Player player = db.Players.Find(id);

            Player player = db.Players
                .Include(p => p.Teams)
                .Where(i => i.ID == id)
                .Single();

            if (player == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Team = new SelectList(db.Teams, "ID", "Name", player.Teams);
            PopulateAssignedTeamData(player);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedTeams)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var playerToUpdate = db.Players
                .Include(p => p.Teams)
                .Where(i => i.ID == id)
                .Single();
            
            try
            {
                UpdatePlayerTeams(selectedTeams, playerToUpdate);

                db.Entry(playerToUpdate).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            

            //ViewBag.TeamID = new SelectList(db.Teams, "ID", "TeamName", playerToUpdate.TeamID);
            PopulateAssignedTeamData(playerToUpdate);
            return View(playerToUpdate);
        }

        // GET: Players/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedTeamData(Player player)
        {
            var allTeams = db.Teams;
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
            ViewBag.Teams = viewModel;
        }

        private void UpdatePlayerTeams(string[] selectedTeams, Player playerToUpdate)
        {
            if (selectedTeams == null)
            {
                playerToUpdate.Teams = new List<Team>();
                return;
            }

            var selectedTeamsHS = new HashSet<string>(selectedTeams);
            var playerTeams = new HashSet<int>
                (playerToUpdate.Teams.Select(t => t.ID));
            foreach (var team in db.Teams)
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
