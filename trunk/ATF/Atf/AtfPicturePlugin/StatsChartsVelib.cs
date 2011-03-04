﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Ming.Atf.Pictures {
  class StatsChartsVelib {
    #region champs
    //private ArrayList dataVelib;
    private Dictionary<String, int> dayToInt;
    private Dictionary<int, String> intToEchelle;
    private Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> statsTabSemaine;
    private Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> statsTabHeure;
    private Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> statsTabJour;
    private ComboBox choiceTimeTop;
    private ComboBox choiceTimeBot;
    private ComboBox choiceGeoBot;
    private ComboBox choiceGeoTop;
    private Panel panTop;
    private Panel panBot;
    private int stationDisplayed;
    private Chart chartBot;
    private Chart chartTop;
    int lastIndexgeotop;
    int lastIndexgeobot;
    int lastIndexTimetop;
    int lastIndexTimebot;

    #endregion

    #region Constructeur
    /*
     * Constructeur
     */
    public StatsChartsVelib(Boolean files) {
      dayToInt = new Dictionary<String, int>();
      intToEchelle = new Dictionary<int, String>();
      intToEchelle.Add( 0, "Par Heures" );
      intToEchelle.Add( 1, "Par jour de la semaine" );
      intToEchelle.Add( 2, "Par semaines" );
      dayToInt.Add( "Monday", 0 );
      dayToInt.Add( "Tuesday", 1 );
      dayToInt.Add( "Wednesday", 2 );
      dayToInt.Add( "Thursday", 3 );
      dayToInt.Add( "Friday", 4 );
      dayToInt.Add( "Saturday", 5 );
      dayToInt.Add( "Sunday", 6 );
      lastIndexgeotop = 0;
      lastIndexgeobot = 0;
      lastIndexTimetop = 0;
      lastIndexTimebot = 0;
      if ( files ) {
        statsTabHeure = (Dictionary<int, Dictionary<int, KeyValuePair<double, double>>>) MySerializer.DeSerializeObject( "tabStationParHeure" );
        statsTabJour = (Dictionary<int, Dictionary<int, KeyValuePair<double, double>>>) MySerializer.DeSerializeObject( "tabStationParJour" );
        //statsTabSemaine = (Dictionary<int, Dictionary<int, KeyValuePair<double, double>>>) MySerializer.DeSerializeObject( "tabStationParSemaine" );
      }
      else {
        //statsTabSemaine = createDicoStationParSemaine();
        statsTabHeure = createDicoStationParHeure();
        statsTabJour = createDicoStationParJour();
      }
      
      
    }

    #endregion

    #region méthodes

    public Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> createDicoStationParHeure() {
      Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> res = new Dictionary<int, Dictionary<int, KeyValuePair<double, double>>>();
      DateTime time = new DateTime(1970,1,1);
      DateTime timeS = new DateTime( 1970, 1, 2 );
      Dictionary<int, KeyValuePair<double, double>> tempdico = null ;
      for ( int k = 0 ; k < 24 ; k++ ) {   
        Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> receivedData = LocalDataBase.getLinesByDateHours(timeS,time, k);
        foreach ( int station in receivedData.Keys ) {
          if ( k == 0 ) {
            tempdico = new Dictionary<int, KeyValuePair<double, double>>();
            tempdico[ k ] = new KeyValuePair<double, double>( receivedData[ station ][ k ].Key, receivedData[ station ][ k ].Value );
            res[ station ] = tempdico;
          }
          else {
            res[ station ][ k ] = new KeyValuePair<double, double>( receivedData[ station ][ k ].Key, receivedData[ station ][ k ].Value );
          }
        }
      }
      MySerializer.SerializeObject( "tabStationParHeure", res );
      return res;
    }

    public Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> createDicoStationParJour() {
      Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> res = new Dictionary<int, Dictionary<int, KeyValuePair<double, double>>>();
      DateTime time = new DateTime( 1970, 1, 1 );
      DateTime timeS = new DateTime( 1970, 1, 2 );
      Dictionary<int, KeyValuePair<double, double>> tempdico = null;

      for ( int k = 1 ; k < 8 ; k++ ) {
        Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> receivedData = LocalDataBase.getLinesByDateDays( timeS, time, k );
        foreach ( int station in receivedData.Keys ) {
          if ( k == 1 ) {
            tempdico = new Dictionary<int, KeyValuePair<double, double>>();
            tempdico[ k ] = new KeyValuePair<double, double>( receivedData[ station ][ k ].Key, receivedData[ station ][ k ].Value );
            res[ station ] = tempdico;
          }
          else {
            res[ station ][ k ] = new KeyValuePair<double, double>( receivedData[ station ][ k ].Key, receivedData[ station ][ k ].Value );
          }
        }
      }
      MySerializer.SerializeObject( "tabStationParJour", res );
      return res;
    } 



    public Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> createDicoStationParSemaine() {
      Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> res = new Dictionary<int, Dictionary<int, KeyValuePair<double, double>>>();
      DateTime time = new DateTime( 1970, 1, 1 );
      DateTime timeS = new DateTime( 1970, 1, 2 );
      Dictionary<int, KeyValuePair<double, double>> tempdico = null;
      for ( int k = 0 ; k < 4 ; k++ ) {
        Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> receivedData = LocalDataBase.getLinesByDateWeeks( timeS, time, k );
        foreach ( int station in receivedData.Keys ) {
          if ( k == 0 ) {
            tempdico = new Dictionary<int, KeyValuePair<double, double>>();
            tempdico[ k ] = new KeyValuePair<double, double>( receivedData[ station ][ k ].Key, receivedData[ station ][ k ].Value );
            res[ station ] = tempdico;
          }
          else {
            res[ station ][ k ] = new KeyValuePair<double, double>( receivedData[ station ][ k ].Key, receivedData[ station ][ k ].Value );
          }
        }
      }
      MySerializer.SerializeObject( "tabStationParSemaine", res );
      return res;
    }

    public Dictionary<int, KeyValuePair<double, double>> StatistiquesParis(String echelle) {
      Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> dataMap;
      if ( echelle == "Heure" ) {
        dataMap = statsTabHeure;
      }
      else if ( echelle == "Jour" ) {
        dataMap = statsTabJour;
      }
      else {
        dataMap = statsTabSemaine;
      }
      Dictionary<int, KeyValuePair<double, double>> res = new Dictionary<int, KeyValuePair<double, double>>();
      Dictionary<int, int> resCount = new Dictionary<int, int>();
      foreach ( int borne in dataMap.Keys ) {
          foreach ( int temps in dataMap[ borne ].Keys ) {
            if ( res.ContainsKey( temps ) ) {
              KeyValuePair<double, double> kvp = new KeyValuePair<double, double>( res[ temps ].Key + dataMap[ borne ][ temps ].Key, res[ temps ].Value + dataMap[ borne ][ temps ].Value );
              res[ temps ] = kvp;
              resCount[ temps ] = resCount[temps] + 1;
            }
            else {
              KeyValuePair<double, double> kvp = new KeyValuePair<double, double>( dataMap[ borne ][ temps ].Key, dataMap[ borne ][ temps ].Value );
              res[ temps ] = kvp;
              resCount[ temps ] = 1;
            }
          }
        }

      foreach ( int cpt in resCount.Keys ) {
        res[ cpt ] = new KeyValuePair<double, double>( res[ cpt ].Key / resCount[cpt], res[ cpt ].Value / resCount[cpt] ); 
      }
      return res;
    }

    public Dictionary<int, KeyValuePair<double, double>> StatistiquesArrondissement(  int Arrondissement, String echelle) {
      Dictionary<int, Dictionary<int, KeyValuePair<double, double>>> dataMap;
      if ( echelle == "Heure" ) {
        dataMap = statsTabHeure;
      }
      else if(echelle == "Jour"){
        dataMap = statsTabJour;
      }
      else{
        dataMap = statsTabSemaine;
      }
      Dictionary<int, KeyValuePair<double, double>> res = new Dictionary<int, KeyValuePair<double, double>>();
      Dictionary<int, int> resCount = new Dictionary<int, int>();
      foreach ( int borne in dataMap.Keys ) {
        //Calculer la moyenne de dispo par heure sur toute la période de la dataMap
        if ( convertStationToDistrict( borne ) == Arrondissement ) {
          foreach ( int temps in dataMap[ borne ].Keys ) {
            if ( res.ContainsKey( temps ) ) {
              KeyValuePair<double, double> kvp = new KeyValuePair<double, double>( res[ temps ].Key + dataMap[ borne ][ temps ].Key, res[ temps ].Value + dataMap[ borne ][ temps ].Value );
              res[ temps ] = kvp;
              resCount[ temps ] = resCount[ temps ] + 1;
            }
            else {
              KeyValuePair<double, double> kvp = new KeyValuePair<double, double>( dataMap[ borne ][ temps ].Key, dataMap[ borne ][ temps ].Value );
              res[ temps ] = kvp;
              resCount[ temps ] = 1;
            }
          }
        }
      }
      foreach ( int cpt in resCount.Keys ) {
        res[ cpt ] = new KeyValuePair<double, double>( res[ cpt ].Key / resCount[ cpt ], res[ cpt ].Value / resCount[ cpt ] );
      }
      
       
      return res;
    }


    #endregion

    #region Graphes
    public Chart createChartStation( int station, String echelle, String type ) {

      Dictionary<int, KeyValuePair<double, double>> statsTab;
      if ( type == "Arrondissement" ) {
        statsTab = this.StatistiquesArrondissement( convertStationToDistrict( station ) ,echelle);
      }
      else if ( type == "Paris" ) {

        statsTab = this.StatistiquesParis(echelle);
      }
      else {
        if ( echelle == "Heure" ) {
          statsTab = this.statsTabHeure[ station ];
        }
        else if ( echelle == "Jour" ) {
            statsTab = this.statsTabJour[ station ];
        }
        else {
          statsTab = this.statsTabSemaine[ station ];        
        }
        
      }
      
      Chart chartStat;
      ChartArea meanArea = new ChartArea();
      Series plusEcart = new Series("EcartType");


      meanArea.AxisY.Title = "Moyenne disponibilité";
      meanArea.AxisX.Title = echelle;
      meanArea.AxisX.TitleFont = new System.Drawing.Font( "Helvetica", 10, System.Drawing.FontStyle.Bold );
      meanArea.AxisY.TitleFont = new System.Drawing.Font( "Helvetica", 10, System.Drawing.FontStyle.Bold );
      meanArea.Name = "StatArea";
      meanArea.AxisX.MajorGrid.Enabled = false;
      Legend legendStat = new Legend();
      Series seriesStat = new Series( "Moyenne disponibilité " +  echelle );



      chartStat = new System.Windows.Forms.DataVisualization.Charting.Chart();
      ((System.ComponentModel.ISupportInitialize) (chartStat)).BeginInit();
      
      
      foreach(int temps in statsTab.Keys){
        seriesStat.Points.AddXY(temps,statsTab[ temps ].Key );
        
      }
      foreach ( int temps in statsTab.Keys ) {
        double[] dataD = new double[] { statsTab[ temps ].Key ,statsTab[ temps ].Key - Math.Sqrt(statsTab[ temps ].Value), statsTab[ temps ].Key + Math.Sqrt(statsTab[ temps ].Value)};
        DataPoint point = new DataPoint(temps,dataD);
        plusEcart.Points.Add(point); 
      }


      seriesStat.Sort( PointSortOrder.Ascending, "X" );
      seriesStat.Color = System.Drawing.Color.Purple;
      plusEcart.Color = System.Drawing.Color.RoyalBlue;
      seriesStat.BorderWidth = 2;
      plusEcart.BorderWidth = 1;
      plusEcart.Sort( PointSortOrder.Ascending, "X" );
      chartStat.Series.Add( seriesStat );
      chartStat.Series.Add( plusEcart );
      chartStat.ChartAreas.Add( meanArea );
      chartStat.BackColor = System.Drawing.Color.Silver;
      meanArea.BackColor = System.Drawing.Color.LightGray;
      legendStat.Name = "legend";
      legendStat.DockedToChartArea = "StatArea";
      //legendStat.Docking = Docking.Bottom;
      //legendStat.Alignment = System.Drawing.StringAlignment.Far;
      Title titre;
      chartStat.Legends.Add( legendStat );
      
      if ( type == "Arrondissement" ) {
        titre = new Title( type + " " + convertStationToDistrict( station ));
      }
      else if ( type == "Paris" ) {
        titre = new Title( type + " ");
      }
      else {
        titre = new Title( type + " " +  station );
      }
      
      titre.Font = new System.Drawing.Font( "Helvetica", 10, System.Drawing.FontStyle.Bold );

      chartStat.Titles.Add( titre );
      chartStat.Size = new System.Drawing.Size( 500, 300 );
      seriesStat.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
      plusEcart.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.ErrorBar;

      ((System.ComponentModel.ISupportInitialize) (chartStat)).EndInit();

      return chartStat;

    }


    public SplitContainer initSplitPanel(int station) {
      stationDisplayed = station;
      SplitContainer resPanel = new SplitContainer();
      resPanel.Size = new System.Drawing.Size( 500, 800 );
      resPanel.Panel1.MinimumSize = new System.Drawing.Size( 500, 350 ); 
      resPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      Panel top = creatLeftTopPanel( station,"Heure","Station" );
      Panel bot = creatLeftBotPanel( station,"Heure","Paris" );
      resPanel.Panel1.Controls.Add( top );
      resPanel.Panel2.Controls.Add( bot );
      top.Padding = new Padding( 15 );
      bot.Padding = new Padding( 15 );
      return resPanel;
    }


    public Panel creatLeftTopPanel(int station,String echelle,String geo) {
      panTop = new Panel();
      panTop.Name = "panTop";
      panTop.BorderStyle = BorderStyle.FixedSingle;
      panTop.Size = new System.Drawing.Size( 500, 330 );
      chartTop = createChartStation( station, echelle, geo );
      panTop.Controls.Add( chartTop );

      choiceTimeTop = createChoiceTimeList();
      choiceTimeTop.SelectedIndexChanged += changeTimeTop;
      choiceGeoTop = createChoiceGeoList();
      choiceGeoTop.SelectedIndexChanged += changeGeoTop;
      panTop.Controls.Add( choiceTimeTop );
      panTop.Controls.Add( choiceGeoTop );
      choiceTimeTop.Location = new System.Drawing.Point( 5, 302 );
      choiceGeoTop.Location = new System.Drawing.Point( 130, 302 );
      return panTop;
    
    }


    public Panel creatLeftBotPanel( int station, String echelle, String geo ) {
      panBot = new Panel();
      panBot.Name = "panBot";
      panBot.Size = new System.Drawing.Size( 500, 330 );
      panBot.BorderStyle = BorderStyle.FixedSingle;
      chartBot = createChartStation( station,echelle,geo );
      panBot.Controls.Add( chartBot );

      choiceTimeBot = createChoiceTimeList();
      choiceTimeBot.SelectedIndexChanged += changeTimeBot;
      choiceGeoBot = createChoiceGeoList();
      choiceGeoBot.SelectedIndexChanged += changeGeoBot;
      panBot.Controls.Add( choiceTimeBot );
      panBot.Controls.Add( choiceGeoBot );
      choiceTimeBot.Location = new System.Drawing.Point( 5, 302 );
      choiceGeoBot.Location = new System.Drawing.Point( 130, 302 );
      return panBot;

    }


    public ComboBox createChoiceTimeList() {
      ComboBox listeRes = new ComboBox();
      listeRes.Items.Add("Heure");
      listeRes.Items.Add("Jour" );
      //listeRes.Items.Add("Semaine");
      
      return listeRes;
    }

    /* Mets a jour la barre de status */
    private void changeGeoTop( object sender, EventArgs args ) {
      String geo;
      String echelle;
      if(choiceGeoTop.SelectedIndex == 0){
        geo = "Station";
      }
      else if ( choiceGeoTop.SelectedIndex == 1 ) {
        geo = "Arrondissement";
      }
      else{
        geo = "Paris";
      }

      if ( lastIndexTimetop == 1 ) {
        echelle = "Jour";
      }
      else if ( lastIndexTimetop == 2 ) {
        echelle = "Semaine";
      }
      else {
        echelle = "Heure";
      }
      lastIndexgeotop = choiceGeoTop.SelectedIndex;
      chartTop.Dispose();
      chartTop = createChartStation( stationDisplayed, echelle, geo );
      panTop.Controls.Add(chartTop  ); 
     
    }

    private void changeGeoBot( object sender, EventArgs args ) {
      String geo;
      String echelle;
      if ( choiceGeoBot.SelectedIndex == 0 ) {
        geo = "Station";
        
      }
      else if ( choiceGeoBot.SelectedIndex == 1 ) {
        geo = "Arrondissement";
      }
      else {
        geo = "Paris";
      }

      if ( lastIndexTimebot == 1 ) {
        echelle = "Jour";
      }
      else if ( lastIndexTimebot == 2 ) {
        echelle = "Semaine";
      }
      else {
        echelle = "Heure";
      }
      lastIndexgeobot = choiceGeoBot.SelectedIndex;
      chartBot.Dispose();
      chartBot = createChartStation( stationDisplayed, echelle, geo );
      panBot.Controls.Add( chartBot );
    }

    private void changeTimeBot( object sender, EventArgs args ) {
      String geo;
      String echelle;
      if ( choiceTimeBot.SelectedIndex == 0 ) {
        echelle = "Heure";
      }
      else if ( choiceTimeBot.SelectedIndex == 1 ) {
        echelle = "Jour";
      }
      else {
        echelle = "Semaine";
      }

      if ( lastIndexgeobot == 1 ) {
        geo = "Arrondissement";
      }
      else if ( lastIndexgeobot == 2 ) {
        geo = "Paris";
      }
      else {
        geo = "Station";
      }

      chartBot.Dispose();
      chartBot = createChartStation( stationDisplayed, echelle, geo );
      panBot.Controls.Add( chartBot );
      
    }

    private void changeTimeTop( object sender, EventArgs args ) {
      String geo;
      String echelle;
      if ( choiceTimeTop.SelectedIndex == 0 ) {
        echelle = "Heure";
      }
      else if ( choiceTimeTop.SelectedIndex == 1 ) {
        echelle = "Jour";
      }
      else {
        echelle = "Semaine";
      }

      if ( lastIndexgeotop == 1 ) {
        geo = "Arrondissement";
      }
      else if ( lastIndexgeotop == 2 ) {
        geo = "Paris";
      }
      else {
        geo = "Station";
      }
      lastIndexTimetop = choiceTimeTop.SelectedIndex;
      chartTop.Dispose();
      chartTop = createChartStation( stationDisplayed, echelle, geo );
      panTop.Controls.Add( chartTop );                           
     
    }

    public ComboBox createChoiceGeoList() {
      ComboBox listeRes = new ComboBox();
      listeRes.Items.Add( "Station" );
      listeRes.Items.Add( "Arrondissement" );
      listeRes.Items.Add( "Paris" );
      return listeRes;
    }


    #endregion

    #region méthodes de service
    /*
     * Convertion de timestamp en DateTime
     */
    private static DateTime ConvertFromUnixTimestamp( double timestamp ) {
      DateTime origin = new DateTime( 1970, 1, 1, 0, 0, 0, 0 );

      return origin.AddSeconds( timestamp );
    }

    /*
     * Converti un numéro de station en arrondissemen
     */
    private static int convertStationToDistrict( int station ) {
      int district = 0;
      String stration = station.ToString();
      int longueur = stration.Length;
      if ( longueur == 5 ) {
        stration = stration.Substring( 0, 2 );
      }
      if ( longueur == 4 ) {
        stration = stration.Substring( 0, 1 );
      }
      district = int.Parse( stration );
      return district;
    }

    #endregion
  }
}