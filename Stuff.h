const int MaxNickname = 24;

typedef char Nickname[MaxNickname];

struct Player
{
	Nickname nickname;
	int gold;
	int winsCount;
	int gamesCount;
	bool premium;
	int reg;
	int lastLogin;
};

bool _cdecl filter(Nickname nickname, int gold, int winsCount, int gamesCount,bool premium, int reg, int lastLogin);
bool _cdecl greaterThen(Player &L, Player &R);