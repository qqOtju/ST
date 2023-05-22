#include <string>
#include <vector>
#include "Stuff.h"
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include <fstream>

using namespace std;

vector<string> SplitStr(string s) {
	vector<string> res;
	string::const_iterator a = s.begin();
	string::const_iterator b = s.end();
	string cur;
	bool inQoutes = false;
	while (a != b)
	{
		switch (*a)
		{
			case ' ': if (!cur.empty()) {
				if (!inQoutes) {
					res.push_back(cur);
					cur.clear();
				}
				else
					cur.push_back(*a);
				} break;
			case '"': inQoutes = !inQoutes; break;
			default: cur.push_back(*a); break;
		}
		a++;
	}
	if (!cur.empty()) res.push_back(cur);
	return res;
}

string PlayerToStr(Player player) {
	string res;
	res = res + "\"" + player.nickname + "\" ";
	res += to_string(player.gold) + " ";
	res += to_string(player.winsCount) + " ";
	res += to_string(player.gamesCount) + " ";
	res += player.premium ? "\"premium\" " : "\"not_premium\" ";
	res += to_string(player.reg) + " ";
	res += to_string(player.lastLogin) + " ";
	return res;
}

Player PlayerFromStr(string str) {
	vector<string> t = SplitStr(str);
	Player res;

	strcpy_s(res.nickname, t[0].c_str());
	res.gold = atoi(t[1].c_str());
	res.winsCount = atoi(t[2].c_str());
	res.gamesCount = atoi(t[3].c_str());
	res.premium = t[4] == "premium";
	res.reg = atoi(t[5].c_str());
	res.lastLogin = atoi(t[6].c_str());
	return res;
}

vector<Player> LoadFromFile(string path) {
	vector<Player> res;
	ifstream file(path);
	string str;
	while (getline(file, str)) res.push_back(PlayerFromStr(str));
	return res;
}

void SaveToFile(string path, vector<Player> data){
	ofstream F(path);
	if (!F.is_open()) return;
	for (int i = 0; i < data.size(); i++) F << PlayerToStr(data[i]) << endl;
	F.close();
}

void Sort(vector<Player> data) {
	int i = 0;
	while (i < data.size()) {
		Player x = data[i];
		int j = i - 1;
		while (j >= 0 && greaterThen(data[j], x)) {
			data[j + 1] = data[j];
			j--;
		}
		data[j + 1] = x;
		i++;
	}
}
int main(){
	//ofstream F(".\\result.csv");
	//if (!F.is_open()) return 0;
	//F << "AAAAAAA";
	//F.close();

	vector<Player> data = LoadFromFile(".\\data.csv");
	cout << data.size() << "items loaded\n";
	vector<Player> res;
	for (int i = 0; i < data.size(); i++)
		if (filter(data[i].nickname, data[i].gold, data[i].winsCount, data[i].gamesCount, data[i].premium, data[i].reg, data[i].lastLogin))
			res.push_back(data[i]);
	cout << res.size() << " Items selected\n";
	Sort(res);
	SaveToFile(".\\result.csv", res);
	return 0;
}