﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Web;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace JSONapi.Models
{
    public class FestivalData
    {

        private ObservableCollection<StageGroup> _stages;

        public ObservableCollection<StageGroup> Stages
        {
            get { return this._stages; }
            set { _stages = value; }
        }
        public  ObservableCollection<StageGroup> GetFestivalData()
        {
            if (this.Stages != null && this._stages.Count != 0)
                return this._stages;
            ObservableCollection<StageGroup> stages = new ObservableCollection<StageGroup>();
            // inladen stage
            // https://blogs.oracle.com/MySqlOnWindows/entry/how_to_using_connector_net
            //		per stage de linups ophalen
            //			per linup de matchende band ophalen
            using (SqlConnection connection = new SqlConnection("Data Source=GHOST\\SQLEXPRESS;Initial Catalog=festivalapp;User ID=SSAUserV2;Password=P@ssw0rd"))
            {
                connection.Open();
                // stage list
                SqlCommand stagesCommand = new SqlCommand("SELECT * FROM stages", connection);
                using (SqlDataReader reader = stagesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StageGroup group = new StageGroup(reader["Id"].ToString(), reader["StageName"].ToString());
                        int i = -1;
                        if (Int32.TryParse(group.UniqueId, out i) && i > 0)
                        {
                            string sql = "SELECT * FROM lineup WHERE Stage = " + i.ToString();
                            SqlConnection conlinup = new SqlConnection("Data Source=GHOST\\SQLEXPRESS;Initial Catalog=festivalapp;User ID=SSAUserV2;Password=P@ssw0rd");
                            conlinup.Open();
                            SqlCommand lineupCommand = new SqlCommand(sql, conlinup);
                            using (SqlDataReader readerlineup = lineupCommand.ExecuteReader())
                            {
                                ObservableCollection<lineupItem> lineups = new ObservableCollection<lineupItem>();
                                while (readerlineup.Read())
                                {
                                    ArtiestItem artiest = new ArtiestItem("-1", "", "ERROR", "ERROR artiest is null", "", "", new List<string>());
                                    string sqlartist = "SELECT * FROM artist WHERE Id=" + readerlineup["Artist"].ToString();
                                   SqlConnection conartist = new SqlConnection("Data Source=GHOST\\SQLEXPRESS;Initial Catalog=festivalapp;User ID=SSAUserV2;Password=P@ssw0rd");
                                    conartist.Open();
                                    SqlCommand artistCommand = new SqlCommand(sqlartist, conartist);
                                    using (SqlDataReader readerartist = artistCommand.ExecuteReader())
                                    {
                                        while (readerartist.Read())
                                        {
                                            List<string> genres = new List<string>();
                                            string sqlGenres = "SELECT genres.GenreNaam FROM artist_genre INNER JOIN genres on artist_genre.GenreID=genres.Id WHERE ArtistID=" + readerlineup["Artist"].ToString();
                                            SqlConnection congenres = new SqlConnection("Data Source=GHOST\\SQLEXPRESS;Initial Catalog=festivalapp;User ID=SSAUserV2;Password=P@ssw0rd");
                                            congenres.Open();
                                            SqlCommand genresCommand = new SqlCommand(sqlGenres, congenres);
                                            using (SqlDataReader readergenres = genresCommand.ExecuteReader())
                                            {
                                                while (readergenres.Read())
                                                {
                                                    genres.Add(readergenres["GenreNaam"].ToString());
                                                }
                                                readergenres.Close();
                                            }
                                            artiest = new ArtiestItem(readerartist["Id"].ToString(), readerartist["Picture"].ToString(), readerartist["Naam"].ToString(), readerartist["Description"].ToString(), readerartist["Twitter"].ToString(), readerartist["Facebook"].ToString(), genres);
                                            congenres.Close();
                                        }
                                        readerartist.Close();
                                        
                                    }
                                    conartist.Close();
                                    //if (artiest == null) artiest = new ArtiestItem("-1", "", "ERROR", "ERROR artiest is null", "", "", new List<string>());
                                    lineupItem lineup = new lineupItem(readerlineup["Id"].ToString(), group.UniqueId, readerlineup["DateOfPlay"].ToString(), readerlineup["Start"].ToString(), readerlineup["Einde"].ToString(), artiest);
                                    lineups.Add(lineup);

                                }
                                group.Items = lineups;
                                readerlineup.Close();
                                conlinup.Close();
                            }
                        }
                        stages.Add(group);

                    }
                    reader.Close();
                }
            }
            return stages;

        }
        public void GeefStages()
        {
            ObservableCollection<StageGroup> stages = this.Stages;
            ObservableCollection<StageGroup> stageTag = new ObservableCollection<StageGroup>();
            
        
        }
    }
    public class ArtiestItem
    {
        public ArtiestItem(String uniqueId, String Picture, String Name, String description, String twitter, String facebook, List<string> genres)
        {
            this.UniqueId = uniqueId;
            this.Picture = Picture;
            this.Name = Name;
            this.Description = description;
            this.Facebook = facebook;
            this.Twitter = twitter;
            this.Genres = genres;
        }

        public string UniqueId { get; private set; }
        public string Picture { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Facebook { get; private set; }
        public string Twitter { get; private set; }
        public List<string> Genres { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class StageGroup
    {
        public StageGroup(String uniqueId, String name)
        {
            this.UniqueId = uniqueId;
            this.Name = name;
            this.Items = new ObservableCollection<lineupItem>();
        }
        public string UniqueId { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<lineupItem> Items { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
    public class lineupItem
    {
        public lineupItem(String uniqueId, string stageId, string dateOfPlay, string start, string end, ArtiestItem artist)
        {
            this.artiest = artist;
            this.UniqueId = uniqueId;
            this.StageId = stageId;
            this.DateOfPlay = dateOfPlay;
            this.Start = start;
            this.Einde = end;

        }
        public ArtiestItem artiest { get; set; }
        public String StageId { get; private set; }
        public string DateOfPlay { get; private set; }
        public string Start { get; private set; }
        public string Einde { get; private set; }
        public string UniqueId { get; private set; }



        public override string ToString()
        {
            return this.artiest.Name + this.StageId;
        }

    }
}