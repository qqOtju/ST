#include "Stuff.h"
#include "string"

bool _cdecl filter(Nickname nickname, int gold, int winsCount, int gamesCount, bool premium, int reg, int lastLogin) {
	return strcmp(nickname, "Qwe") == 0 && gold > 100 && premium == true;
}

bool _cdecl greaterThen(Player& L, Player& R) {
	return &L.winsCount > &R.winsCount;
}