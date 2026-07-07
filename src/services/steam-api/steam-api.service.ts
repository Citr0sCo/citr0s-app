import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {map, Observable} from "rxjs";
import {environment} from "../../environments/environment";
import {ISteamUserProfile} from "./types/steam-user-profile.type";
import {ISteamUserProfileDecoration} from "./types/steam-user-profile-decoration.type";
import {ISteamUserActivity} from "./types/steam-user-activity.type";
import {ISteamGameSummary} from "./types/steam-game-summary.type";

@Injectable()
export class SteamApiService {

    private _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public getUserProfile(steamId: string): Observable<ISteamUserProfile> {
        return this._httpClient.get<any>(`${environment.apiBaseUrl}/api/steam/${steamId}`)
            .pipe(
                map((response: any): ISteamUserProfile => {
                    return {
                        avatarUrl: response.AvatarUrl,
                        lastOnline: response.LastOnline,
                        profileUrl: response.ProfileUrl,
                        status: response.Status,
                        username: response.Username,
                    };
                })
            );
    }

    public getUserProfileDecoration(steamId: string): Observable<ISteamUserProfileDecoration> {
        return this._httpClient.get<any>(`${environment.apiBaseUrl}/api/steam/${steamId}/decoration`)
            .pipe(
                map((response: any): ISteamUserProfileDecoration => {
                    return {
                        avatarBorder: response.AvatarBorder,
                        profileBackground: response.ProfileBackground
                    };
                })
            );
    }

    public getUserActivity(steamId: string): Observable<ISteamUserActivity> {
        return this._httpClient.get<any>(`${environment.apiBaseUrl}/api/steam/${steamId}/activity`)
            .pipe(
                map((response: any): ISteamUserActivity => {
                    return {
                        total: response.Total,
                        games: response.Games.map(((game: any): ISteamGameSummary => {
                            return {
                                appId: game.AppId,
                                iconUrl: game.IconUrl,
                                name: game.Name,
                                playtimeForeverInMinutes: game.PlaytimeForeverInMinutes,
                                playtimeLastTwoWeeksInMinutes: game.PlaytimeLastTwoWeeksInMinutes
                            };
                        })),
                    };
                })
            );
    }
}