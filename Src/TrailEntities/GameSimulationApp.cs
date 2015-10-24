﻿using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Receiver - The main logic will be implemented here and it knows how to perform the necessary actions.
    /// </summary>
    public sealed class GameSimulationApp : SimulationApp, IGameSimulation
    {
        /// <summary>
        ///     Manages weather, temperature, humidity, and current grazing level for living animals.
        /// </summary>
        private ClimateSim _climate;

        /// <summary>
        ///     Manages time in a linear since from the provided ticks in base simulation class. Handles days, months, and years.
        /// </summary>
        private TimeSim _time;

        /// <summary>
        ///     Current vessel which the player character and his party are traveling inside of, provides means of transportation
        ///     other than walking.
        /// </summary>
        private Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimulationApp" /> class.
        /// </summary>
        public GameSimulationApp()
        {
            _time = new TimeSim(1848, Months.March, 1, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            _climate = new ClimateSim(this, ClimateClassification.Moderate);
            TrailSim = new TrailSim();
            TotalTurns = 0;
            _vehicle = new Vehicle(this);
        }

        public TrailSim TrailSim { get; private set; }

        public static GameSimulationApp Instance { get; private set; }

        public ITimeSimulation Time
        {
            get { return _time; }
        }

        public IClimateSimulation Climate
        {
            get { return _climate; }
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        public uint TotalTurns { get; private set; }

        /// <summary>
        ///     Attaches the traveling mode and removes the new game mode if it exists, this begins the simulation down the trail
        ///     path and all the points of interest on it.
        /// </summary>
        /// <param name="newGameInfo">User data object that was passed around the new game mode and populated by user selections.</param>
        public override void StartGame(NewGameInfo newGameInfo)
        {
            base.StartGame(newGameInfo);

            // Complain if there is no players to add to the vehicle.
            if (newGameInfo.PlayerNames.Count <= 0)
                throw new InvalidOperationException("Cannot create vehicle with no people in new game info user data!");

            // Clear out any data amount items, monies, people that might have been in the vehicle.
            // NOTE: Sets starting monies, which was determined by player profession selection.
            Vehicle.ResetVehicle(newGameInfo.StartingMonies);

            // Add all the player data we collected from attached game mode states.
            var crewNumber = 1;
            foreach (var name in newGameInfo.PlayerNames)
            {
                // First name in list is always the leader.
                var isLeader = newGameInfo.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                Vehicle.AddPerson(new Person(newGameInfo.PlayerProfession, name, isLeader));
                crewNumber++;
            }

            // Set the starting month to match what the user selected.
            Time.SetMonth(newGameInfo.StartingMonth);
        }

        public void TakeTurn()
        {
            TotalTurns++;
            _time.TickTime();
        }

        /// <summary>
        ///     Prints game mode specific text and options.
        /// </summary>
        protected override string OnTickTUI()
        {
            // Spinning ticker that shows activity, lets us know if application hangs or freezes.
            var tui = new StringBuilder();
            tui.Append($"\r[ {TimerTickPhase} ] - ");

            // Keeps track of active mode name and active mode current state name for debugging purposes.
            tui.Append(ActiveMode?.CurrentState != null
                ? $"Mode: {ActiveModeName}({ActiveMode.CurrentState}) - "
                : $"Mode: {ActiveModeName}(NO STATE) - ");

            // Total number of turns that have passed in the simulation.
            tui.Append($"Turns: {TotalTurns.ToString("D4")}\n");

            // Prints game mode specific text and options. This typically is menus from commands, or states showing some information.
            tui.Append($"{base.OnTickTUI()}\n");

            // Only print and accept user input if there is a game mode and menu system to support it.
            if (AcceptingInput)
            {
                // Allow user to see their input from buffer.
                tui.Append($"User Input: {InputBuffer}");
            }

            // Outputs the result of the string builder to TUI builder above.
            return tui.ToString();
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        public override void OnInputBufferReturned(string returnedLine)
        {
            // Pass command along to currently active game mode if it exists.
            ActiveMode?.SendInputBuffer(returnedLine.Trim());
        }

        /// <summary>
        ///     Creates new instance of game simulation. Complains if instance already exists.
        /// </summary>
        public static void Create()
        {
            if (Instance != null)
                throw new InvalidOperationException(
                    "Unable to create new instance of game simulation app since it already exists!");

            Instance = new GameSimulationApp();
        }

        public override void OnDestroy()
        {
            // Unhook delegates from events.
            if (_time != null)
            {
                _time.DayEndEvent -= TimeSimulation_DayEndEvent;
                _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
                _time.YearEndEvent -= TimeSimulation_YearEndEvent;
                _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;
            }

            // Destroy all instances.
            _time = null;
            _climate = null;
            TrailSim = null;
            TotalTurns = 0;
            _vehicle = null;
            Instance = null;

            base.OnDestroy();
        }

        protected override void OnFirstTimerTick()
        {
            base.OnFirstTimerTick();

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            AddMode(ModeType.NewGame);
        }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="modeType">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        protected override IMode OnModeChanging(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.Travel:
                    return new TravelingMode();
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode();
                case ModeType.Hunt:
                    return new HuntingMode();
                case ModeType.Landmark:
                    return new LandmarkMode();
                case ModeType.NewGame:
                    return new NewGameMode();
                case ModeType.RandomEvent:
                    return new RandomEventMode();
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode();
                case ModeType.Settlement:
                    return new SettlementMode();
                case ModeType.Store:
                    return new StoreMode();
                case ModeType.Trade:
                    return new TradingMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(modeType), modeType, null);
            }
        }

        private void TimeSimulation_SpeedChangeEvent()
        {
            //Console.WriteLine("Travel pace changed to " + _vehicle.Pace);
        }

        private void TimeSimulation_YearEndEvent(uint yearCount)
        {
            //Console.WriteLine("Year end!");
        }

        private void TimeSimulation_DayEndEvent(uint dayCount)
        {
            _climate.TickClimate();
            Vehicle.UpdateVehicle();
            TrailSim.ReachedPointOfInterest();
            _vehicle.DistanceTraveled += (uint) Vehicle.Pace;

            //Console.WriteLine("Day end!");
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
            //Console.WriteLine("Month end!");
        }
    }
}