using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameMechanics;
using System.ComponentModel;
using Villain;
using System.Threading;

namespace WPFapplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Player WinnerLastHand;

        private AI Villain;

        private Dealer _mrBrown;
        public Dealer Mr_Brown
        {
            get
            {                
                return _mrBrown;
            }
            set
            {
                _mrBrown = value;
                OnPropertyChanged("dealer");
            }
        }

        // Change everything according to dealer
        private void OnPropertyChanged(string p)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (p == "dealer")
                UpdateTable();
        }

        private void UpdateTable()
        {
            if(_mrBrown.pot > 0)
            {
                Pot.Visibility = Visibility.Visible;
                Pot.Text = "Total pot: " + _mrBrown.pot.ToString();
            }
            else
                Pot.Visibility = Visibility.Hidden;

            // If one of the players has changed potcommit
            if (!(pcht.Text == (Mr_Brown.players[0].PotCommit + Mr_Brown.players[1].PotCommit).ToString() &&
                pcvt.Text == (Mr_Brown.players[0].PotCommit + Mr_Brown.players[1].PotCommit).ToString()))
            {
                if (pcvb.Background == (SolidColorBrush)new BrushConverter().ConvertFromString("BurlyWood"))
                {
                    if (_mrBrown.players[0].PotCommit > 0)
                    {
                        pchb.Visibility = Visibility.Visible;
                        pcht.Text = _mrBrown.players[0].PotCommit.ToString();
                    }
                    else
                        pchb.Visibility = Visibility.Hidden;

                    if (_mrBrown.players[1].PotCommit > 0)
                    {
                        pcvb.Visibility = Visibility.Visible;
                        pcvt.Text = _mrBrown.players[1].PotCommit.ToString();
                    }
                    else
                        pcvb.Visibility = Visibility.Hidden;
                }
            }


            if (_mrBrown.dealer_button.PlayerID == PLAYER.Computer)
            {
                VillainDealer.Visibility = Visibility.Visible;
                HeroDealer.Visibility = Visibility.Hidden; 
            }
            if (_mrBrown.dealer_button.PlayerID == PLAYER.User)
            {
                VillainDealer.Visibility = Visibility.Hidden;
                HeroDealer.Visibility = Visibility.Visible; 
            }

            if (_mrBrown.players[0].HoleCards != null && _mrBrown.players[0].HoleCards.Count > 0)
            {
                Hero1.Source = _mrBrown.players[0].HoleCards[0].GetImageSrc();
                Hero2.Source = _mrBrown.players[0].HoleCards[1].GetImageSrc();

                Hero1.Visibility = Visibility.Visible;
                Hero2.Visibility = Visibility.Visible;
                Villain1.Visibility = Visibility.Visible;
                Villain2.Visibility = Visibility.Visible;
            }
            else
            {
                Hero1.Visibility = Visibility.Hidden;
                Hero2.Visibility = Visibility.Hidden;
                Villain1.Visibility = Visibility.Hidden;
                Villain2.Visibility = Visibility.Hidden;
            }

            HeroStack.Text = _mrBrown.players[0].StackSize.ToString();
            VillainStack.Text = _mrBrown.players[1].StackSize.ToString();

            if (_mrBrown.c.Count == 5)
            {
                River.Visibility = Visibility.Visible;
                River.Source = _mrBrown.c[4].GetImageSrc();
            }
            else if (_mrBrown.c.Count == 4)
            {
                Turn.Visibility = Visibility.Visible;
                Turn.Source = _mrBrown.c[3].GetImageSrc();

                River.Visibility = Visibility.Hidden;
            }
            else if (_mrBrown.c.Count == 3)
            {
                Flop1.Visibility = Visibility.Visible;
                Flop2.Visibility = Visibility.Visible;
                Flop3.Visibility = Visibility.Visible;

                Flop1.Source = _mrBrown.c[0].GetImageSrc();
                Flop2.Source = _mrBrown.c[1].GetImageSrc();
                Flop3.Source = _mrBrown.c[2].GetImageSrc();

                Turn.Visibility = Visibility.Hidden;
                River.Visibility = Visibility.Hidden;
            }
            else
            {
                Flop1.Visibility = Visibility.Hidden;
                Flop2.Visibility = Visibility.Hidden;
                Flop3.Visibility = Visibility.Hidden;
                Turn.Visibility = Visibility.Hidden;
                River.Visibility = Visibility.Hidden;
            }
        }

        // GUI thing
        private int betStep = 0;
        private bool noBetPlaced = false;

        // Initialize
        public MainWindow()
        {
      //      Uri uri = new Uri(@"..\..\Images\Background.png", UriKind.Relative);
      //      PngBitmapDecoder decoder2 = new PngBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {

            int begin_stack = 1500;

            // init players
            List<Player> Participants = new List<Player>();
            foreach (PLAYER p in Enum.GetValues(typeof(PLAYER)))
                Participants.Add(new Player(p, begin_stack));

            // init dealer
            Mr_Brown = new Dealer(Participants);

            Villain = new AI();
        }

        // New hand (eventually could be automatic)
        private void New_Hand_clicked(object sender, RoutedEventArgs e)
        {
            if ((string)newhand.Content == "New Game")
            {
                InitializeGame();
                newhand.Content = "New Hand";
                return;
            }
            newhand.IsEnabled = false;
            Villain1.Source = Card.BackSrc();
            Villain2.Source = Card.BackSrc();
            if(Mr_Brown.players[0].HoleCards != null)
                Mr_Brown.WrapUp(WinnerLastHand);
            SetupHand();
        }

        // Wrapper for entire hand workflow
        public void SetupHand()
        {
            villainAct.Visibility = Visibility.Hidden;
            // If one player has won
            if(Mr_Brown.players[0].StackSize == 0 || Mr_Brown.players[0].StackSize == 1)
            {
                return;
            }

            winner_pane.Visibility = Visibility.Hidden;
            // Preflop sorting
            Mr_Brown.DealHoleCards();
            newhand.Visibility = Visibility.Hidden;
            newhand.IsEnabled = true;

           // Mr_Brown.ChangeDealerButton();
            Mr_Brown.whosturn = Mr_Brown.dealer_button;
            Mr_Brown.AcceptBlinds();
            UpdateTable();

            SolidColorBrush burly = (SolidColorBrush)new BrushConverter().ConvertFromString("BurlyWood");
            pcvb.Background = burly;
            pchb.Background = burly;

            // Begin playing the hand
            BetRound();
        }

        // Bet round for everything
        public void BetRound()
        {
            if (betStep == 0 && Mr_Brown.c.Count > 0)
                Mr_Brown.whosturn = Mr_Brown.OtherPlayer(Mr_Brown.dealer_button);
            else if (betStep > 0)
                Mr_Brown.ChangeTurn();

            if (ContinueBetting())
            {
                betStep++;
                AskAction();
            }
            //Ready for FLOP, start over
            else if (Mr_Brown.c.Count == 0 && EqualPotCommits())
            {
                betStep = 0;
                Mr_Brown.Flop();
                BetRound();
            }
            //TURN
            else if (Mr_Brown.c.Count == 3 && EqualPotCommits())
            {
                betStep = 0;
                Mr_Brown.Turn();
                BetRound();
            }
            else if (Mr_Brown.c.Count == 4 && EqualPotCommits())
            {
                betStep = 0;
                Mr_Brown.River();
                BetRound();
            }
            else
                WrapUp();
        }

        private void WrapUp()
        {
            betStep = 0;
            noBetPlaced = false;
            UpdateTable();

            WinnerLastHand = Mr_Brown.WhoWins();
            // also deals with buttons
            DisplayOutcomeGUI(WinnerLastHand);
        }

        private bool EqualPotCommits()
        {
            return Mr_Brown.players[0].PotCommit == Mr_Brown.players[1].PotCommit;
        }

        private int PotCommitDifference()
        {
            return Math.Abs(Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit);
        }

        private bool ContinueBetting()
        {
            if (betStep < 2 || !noBetPlaced)
                return true;
            return false;
        }

        private void AskAction()
        {
            if (Mr_Brown.whosturn.PotCommit < Mr_Brown.OtherPlayer(Mr_Brown.whosturn).PotCommit)
                AskFoldCallRaise();

            else
                AskCheckRaise();
                
        }

        private void AskCheckRaise()
        {
            // Players turn
            if (Mr_Brown.whosturn.PlayerID == PLAYER.User)
                CheckRaiseGUI();
            // Computers turn
            else
            {
                NoButtonsGUI();
                GetMoveFromAI(Mr_Brown);
            }
        }

        private void AskFoldCallRaise()
        {
            if (Mr_Brown.whosturn.PlayerID == PLAYER.User)
                FoldCallRaiseGUI();
            else
            {
                NoButtonsGUI();
                GetMoveFromAI(Mr_Brown);
            }
        }

        public void GetMoveFromAI(Dealer Mr_Brown)
        {
            BackgroundWorker bgWorker;
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

            bgWorker.RunWorkerAsync();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Villain.GetMove(Mr_Brown, betStep);
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                int amount = (int)e.Result;
                Mr_Brown.AcceptBet(Mr_Brown.players[1], amount);
                // If player has more invested than comp, comp folded/checked/called
                if (Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit >= 0)
                    noBetPlaced = true;
                else
                    noBetPlaced = false;

                if (Mr_Brown.players[0].PotCommit - Mr_Brown.players[1].PotCommit > 0)
                    betStep = 100;
               
                VillainActionGUI(amount);

                BackgroundWorker bgWorker;
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Wait);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WaitCompleted);

                bgWorker.RunWorkerAsync();
          //      BetRound();
            }
        }

        void bgWorker_Wait(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        void bgWorker_WaitCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                BetRound();
            }
        }

        private bool IsPlayersFirstTurn()
        {
            return (Mr_Brown.players[0].PotCommit == 10 && Mr_Brown.dealer_button.PlayerID == PLAYER.User)
                || (Mr_Brown.players[0].PotCommit == 20 && Mr_Brown.dealer_button.PlayerID != PLAYER.User);
        }

        #region GUI stuff
        /// <summary>
        /// This shows the previous action of the villain
        /// </summary>
        /// <param name="amount"></param>
        private void VillainActionGUI(int amount)
        {
            string s;
            if (amount == 0 && EqualPotCommits())
                s = "CHECK";
            else if (amount == 0 && !EqualPotCommits())
                s = "FOLD";
            else if (EqualPotCommits())
                s = "CALL " + amount.ToString();
            else if (PotCommitDifference() == amount)
                s = "BET " + amount.ToString();
            else
                s = "RAISE WITH " + (PotCommitDifference()).ToString();

            villainActText.Text = s;
            villainAct.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This shows the winner of the hand
        /// </summary>
        /// <param name="winner"></param>
        private void DisplayOutcomeGUI(Player winner)
        {
            HandEvaluator h = new HandEvaluator(winner.HoleCards);
            winner_pane.Text = winner == null ? "Split pot" : winner.PlayerID.ToString() + " wins";

            // If showdown
            if(Mr_Brown.c.Count == 5 && EqualPotCommits())
            {
                Villain1.Source = Mr_Brown.players[1].HoleCards[0].GetImageSrc();
                Villain2.Source = Mr_Brown.players[1].HoleCards[1].GetImageSrc();
                winner_pane.Text += "\n" + h.rank.ToString();
            }
            
            winner_pane.Visibility = Visibility.Visible;

            SolidColorBrush redBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("Red");
            SolidColorBrush greenBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("Green");

            if (winner.PlayerID == PLAYER.User)
            {
                pcht.Text = (Mr_Brown.players[0].PotCommit + Mr_Brown.players[1].PotCommit).ToString();
                pcvt.Text = "0";
                pchb.Background = greenBrush;
                pcvb.Background = redBrush;
            }
            else
            {
                pcvt.Text = (Mr_Brown.players[0].PotCommit + Mr_Brown.players[1].PotCommit).ToString();
                pcht.Text = "0";
                pchb.Background = redBrush;
                pcvb.Background = greenBrush;
            }

            //NoButtonsGUI();
            if (Mr_Brown.players[0].StackSize != 0 && Mr_Brown.players[1].StackSize != 0)
                newhand.Visibility = Visibility.Visible;

            else
            {
                string winnerstring = Mr_Brown.players[0].StackSize == 0 ? Mr_Brown.players[1].PlayerID.ToString() : Mr_Brown.players[0].PlayerID.ToString();
                villainActText.Text = "Game over, " + winnerstring + " won!";
                newhand.Content = "New Game";
                newhand.Visibility = Visibility.Visible;
            }

            UpdateTable();
        }

        /// <summary>
        /// Disables all buttons (when opponent's turn)
        /// </summary>
        public void NoButtonsGUI()
        {
            fold.Visibility = Visibility.Hidden;
            check_call.Visibility = Visibility.Hidden;
            raise.Visibility = Visibility.Hidden;
            villainAct.Visibility = Visibility.Hidden;
            UpdateTable();
            UpdateLayout();
        }

        /// <summary>
        /// Enables buttons for checking and raising
        /// </summary>
        public void CheckRaiseGUI()
        {
            RaiseButton.IsEnabled = true;
            fold.Visibility = Visibility.Hidden;
            check_call.Visibility = Visibility.Visible;
            check_call.Content = "Check";
            raise.Visibility = Visibility.Visible;

            int short_stack = Mr_Brown.players[0].StackSize < Mr_Brown.players[1].StackSize ?
                Mr_Brown.players[0].StackSize : Mr_Brown.players[1].StackSize;

            // Takes care of the raise slider min and max values
            if (short_stack > Mr_Brown.small_blind * 2)
            {
                RaiseSlider.Minimum = Mr_Brown.small_blind * 2;
                RaiseSlider.Maximum = short_stack;
            }
            else
            {
                RaiseSlider.Minimum = short_stack;
                RaiseSlider.Maximum = short_stack;
            }
            if (RaiseSlider.Maximum == 0)
                RaiseButton.IsEnabled = false;

            UpdateTable();
        }

        /// <summary>
        /// Enables buttons for when a bet/raise is placed against the player
        /// </summary>
        public void FoldCallRaiseGUI()
        {
            RaiseButton.IsEnabled = true;
            fold.Visibility = Visibility.Visible;
            check_call.Visibility = Visibility.Visible;
            check_call.Content = "Call " + (Mr_Brown.players[1].PotCommit - Mr_Brown.players[0].PotCommit).ToString();
            raise.Visibility = Visibility.Visible;

            int prevBet = Mr_Brown.players[1].PotCommit - Mr_Brown.players[0].PotCommit;

            int short_stack = Mr_Brown.players[0].StackSize - prevBet < Mr_Brown.players[1].StackSize ?
                Mr_Brown.players[0].StackSize : Mr_Brown.players[1].StackSize;

            if (short_stack > Mr_Brown.small_blind * 2)
            {
                
                RaiseSlider.Minimum = prevBet + (Mr_Brown.small_blind * 2);
                RaiseSlider.Maximum = prevBet + short_stack;
            }
            else if(short_stack > 0)
            {
                RaiseSlider.Minimum = short_stack;
                RaiseSlider.Maximum = short_stack;
            }
            // no more raises, one of the two already is all in
            else 
            {
                RaiseSlider.Minimum = 0;
                RaiseSlider.Maximum = 0;
                RaiseButton.IsEnabled = false;
            }

            UpdateTable();
        }
        #endregion

        


        private void Check_Call_clicked(object sender, RoutedEventArgs e)
        {
            int bet = Mr_Brown.players[1].PotCommit - Mr_Brown.players[0].PotCommit;

            if (bet == 0)
                Villain.UpdateMove(MOVE.check, DeterminePhase(Mr_Brown), betStep, IsPlayersFirstTurn());
            else
                Villain.UpdateMove(MOVE.call, DeterminePhase(Mr_Brown), betStep, IsPlayersFirstTurn());

            NoButtonsGUI();
            noBetPlaced = true;
            Mr_Brown.AcceptBet(Mr_Brown.players[0], bet);

            BetRound();
        }

        private void Fold_clicked(object sender, RoutedEventArgs e)
        {
            Villain.UpdateMove(MOVE.fold, DeterminePhase(Mr_Brown), betStep, IsPlayersFirstTurn());
            betStep = 100;
            noBetPlaced = true;
            NoButtonsGUI();
            Mr_Brown.AcceptBet(Mr_Brown.players[0], 0);
            BetRound();
        }

 

        public void RaiseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom = e.Delta > 0 ? -20 : 20;
            if (zoom < 0)
                RaiseSlider.Value = RaiseSlider.Value + zoom < 0 ? 0 : RaiseSlider.Value + zoom;
            else
                RaiseSlider.Value = RaiseSlider.Value + zoom > RaiseSlider.Maximum ? RaiseSlider.Maximum : RaiseSlider.Value + zoom;
            e.Handled = true;
            //Slider
        }

        private void RaiseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int StackSize = Convert.ToInt32(HeroStack.Text);
            if (e.NewValue >= 19.5 || e.NewValue == StackSize)
                RaiseButton.IsEnabled = true;
            else
                RaiseButton.IsEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseButton_Click(object sender, RoutedEventArgs e)
        {
            Villain.UpdateMove(MOVE.bet_raise, DeterminePhase(Mr_Brown), betStep, IsPlayersFirstTurn());
            NoButtonsGUI();
            noBetPlaced = false;
            Mr_Brown.AcceptBet(Mr_Brown.players[0], (int)RaiseSlider.Value);
            
            BetRound();
        }


        private GAME_STATE DeterminePhase(Dealer Mr_Brown)
        {
            if (Mr_Brown.c.Count == 0)
                return GAME_STATE.preflop;
            if (Mr_Brown.c.Count == 3)
                return GAME_STATE.flop;
            if (Mr_Brown.c.Count == 4)
                return GAME_STATE.turn;
            else
                return GAME_STATE.river;
        }
    }

    public class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)Math.Round((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}