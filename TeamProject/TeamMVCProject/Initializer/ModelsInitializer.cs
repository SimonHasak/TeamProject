using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TeamMVCProject.Context;
using TeamMVCProject.Models;

namespace TeamMVCProject.Initializer
{
    public class ModelsInitializer : DropCreateDatabaseIfModelChanges<TeamsPlayersContext>
    {
        protected override void Seed(TeamsPlayersContext context)
        {
            var players = new List<Player>
            {
                new Player{Name="Roman Romanovic", Description="Best football player with great results in math.", Teams = new List<Team>()},
                new Player{Name="Igor Solovic", Description="Scientist with good genes for basketball.", Teams = new List<Team>()},
                new Player{Name="Martina Osova", Description="Dancer and fotball player.", Teams = new List<Team>()},
                new Player{Name="Osol Drtic", Description="Basketball player caught up in Biology classes.", Teams = new List<Team>()},
                new Player{Name="Ivana Ivanicova", Description="Football and basketball player.", Teams = new List<Team>()},
                new Player{Name="Milan Sulok", Description="Biology, Math and Chemistry interests.", Teams = new List<Team>()}
            };

            players.ForEach(p => context.Players.Add(p));
            context.SaveChanges();

            var teams = new List<Team>
            {
                new Team{Name="Fotball", Description="Team for playing fotball.", Players = new List<Player>()},
                new Team{Name="Math", Description="Team for learning math.", Players = new List<Player>()},
                new Team{Name="Basketball", Description="Team for playing basketball.", Players = new List<Player>()},
                new Team{Name="Dancing", Description="Team for dancing.", Players = new List<Player>()},
                new Team{Name="Biology", Description="Team for learning biology.", Players = new List<Player>()},
                new Team{Name="Chemistry", Description="Team for learning chemistry.", Players = new List<Player>()}
            };

            teams.ForEach(p => context.Teams.Add(p));
            context.SaveChanges();

            players[0].Teams.Add(teams[0]);
            players[0].Teams.Add(teams[1]);
            players[1].Teams.Add(teams[2]);
            players[1].Teams.Add(teams[1]);
            players[1].Teams.Add(teams[4]);
            players[1].Teams.Add(teams[5]);
            players[2].Teams.Add(teams[0]);
            players[2].Teams.Add(teams[3]);
            players[3].Teams.Add(teams[2]);
            players[3].Teams.Add(teams[4]);
            players[4].Teams.Add(teams[0]);
            players[4].Teams.Add(teams[2]);
            players[5].Teams.Add(teams[1]);
            players[5].Teams.Add(teams[4]);
            players[5].Teams.Add(teams[5]);

            teams[0].Players.Add(players[0]);
            teams[0].Players.Add(players[2]);
            teams[0].Players.Add(players[4]);
            teams[1].Players.Add(players[0]);
            teams[1].Players.Add(players[1]);
            teams[1].Players.Add(players[5]);
            teams[2].Players.Add(players[1]);
            teams[2].Players.Add(players[3]);
            teams[2].Players.Add(players[4]);
            teams[3].Players.Add(players[2]);
            teams[4].Players.Add(players[1]);
            teams[4].Players.Add(players[3]);
            teams[4].Players.Add(players[5]);
            teams[5].Players.Add(players[1]);
            teams[5].Players.Add(players[5]);

            context.SaveChanges();
        }
    }
} 