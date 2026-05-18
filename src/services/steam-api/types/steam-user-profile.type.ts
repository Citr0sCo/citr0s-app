import {SteamUserStatus} from "./steam-user-status.type";

export interface ISteamUserProfile {
    avatarUrl: string;
    lastOnline: Date;
    profileUrl: string;
    status: SteamUserStatus;
    username: string;
}