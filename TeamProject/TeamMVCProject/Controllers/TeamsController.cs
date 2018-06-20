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
using TeamMVCProject.ViewModels;

namespace TeamMVCProject.Controllers
{
    public class TeamsController : Controller
    {
        private TeamsPlayersContext db = new TeamsPlayersContext();

        // GET: Teams
        public async Task<ActionResult> Index()
        {
            return View(await db.Teams.Include(p => p.Players).ToListAsync());        
        }

        // GET: Teams/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "TeamName");
            var team = new Team();
            team.Players = new List<Player>();
            PopulateAssignedTeamData(team);
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Description")] Team team, string[] selectedPlayers)
        {
            if (selectedPlayers != null)
            {
                team.Players = new List<Player>();
                foreach (var player in selectedPlayers)
                {
                    var playerToAdd = await db.Players.FindAsync(int.Parse(player));
                    team.Players.Add(playerToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                db.Teams.Add(team);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateAssignedTeamData(team);
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Team team = db.Teams.Find(id);db.Players.Include(p => p.Teams).ToListAsync()
            Team team = await db.Teams
                .Include(p => p.Players)
                .Where(c => c.ID == id)
                .SingleAsync();

            if (team == null)
            {
                return HttpNotFound();
            }
            PopulateAssignedTeamData(team);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, string[] selectedPlayers)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var teamToUpdate = await db.Teams
                .Include(p => p.Players)
                .Where(i => i.ID == id)
                .SingleAsync();

            try
            {
                UpdateTeamPlayers(selectedPlayers, teamToUpdate);

                db.Entry(teamToUpdate).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }


            //ViewBag.TeamID = new SelectList(db.Teams, "ID", "TeamName", playerToUpdate.TeamID);
            PopulateAssignedTeamData(teamToUpdate);
            return View(teamToUpdate);
        }

        private void PopulateAssignedTeamData(Team team)
        {
            var allPlayers = db.Players;
            var teamPlayers = new HashSet<int>(team.Players.Select(t => t.ID));
            var viewModel = new List<PlayerTeamVM>();
            foreach (var player in allPlayers)
            {
                viewModel.Add(new PlayerTeamVM
                {
                    ID = player.ID,
                    Name = player.Name,
                    isAssigned = teamPlayers.Contains(player.ID)
                });
            }
            ViewBag.Players = viewModel;
        }

        private void UpdateTeamPlayers(string[] selectedPlayers, Team teamToUpdate)
        {
            if (selectedPlayers == null)
            {
                teamToUpdate.Players = new List<Player>();
                return;
            }

            var selectedPlayersHS = new HashSet<string>(selectedPlayers);
            var teamPlayers = new HashSet<int>
                (teamToUpdate.Players.Select(p => p.ID));
            foreach (var player in db.Players)
            {
                if (selectedPlayersHS.Contains(player.ID.ToString()))
                {
                    if (!teamPlayers.Contains(player.ID))
                    {
                        teamToUpdate.Players.Add(player);
                    }
                }
                else
                {
                    if (teamPlayers.Contains(player.ID))
                    {
                        teamToUpdate.Players.Remove(player);
                    }
                }
            }
        }

        // GET: Teams/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Team team = await db.Teams.FindAsync(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("Index");
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
