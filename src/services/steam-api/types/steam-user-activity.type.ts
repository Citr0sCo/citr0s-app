import {ISteamGameSummary} from "./steam-game-summary.type";

export interface ISteamUserActivity {
    total: number;
    games: Array<ISteamGameSummary>
}