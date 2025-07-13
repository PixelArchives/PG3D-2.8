public class GlobalGameController
{
	private static int _currentLevel = -1;

	private static int _allLevelsCompleted;

	private static int score;

	public static int NumOfLevels
	{
		get
		{
			return 8;
		}
	}

	public static int currentLevel
	{
		get
		{
			return _currentLevel;
		}
		set
		{
			_currentLevel = value;
		}
	}

	public static int AllLevelsCompleted
	{
		get
		{
			return _allLevelsCompleted;
		}
		set
		{
			_allLevelsCompleted = value;
		}
	}

	public static int ZombiesInWave
	{
		get
		{
			return 4;
		}
	}

	public static int EnemiesToKill
	{
		get
		{
			return 40 + 40 * AllLevelsCompleted;
		}
	}

	public static int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}

	public static int SimultaneousEnemiesOnLevelConstraint
	{
		get
		{
			return 25;
		}
	}

	public static void ResetParameters()
	{
		currentLevel = 0;
		AllLevelsCompleted = 0;
	}
}
