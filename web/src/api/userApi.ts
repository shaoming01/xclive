import {R} from "@/utils/R";
import {apiHelper} from "@/utils/apiHelper";
import {IUserLoginInfo} from "@/types/interfaces";

export class userApi {
    static async login(username: string | undefined, password: string | undefined,): Promise<R<IUserLoginInfo>> {
        let url = "/api/user/login";
        let par = {
            username: username,
            password: password,
        };

        return apiHelper.request(url, par);
    }

    static async info(token: string | undefined): Promise<R> {
        let url = "/api/user/info";
        let par = {
            token: token,
        };

        return apiHelper.request(url, par);
    }

    static async logout(token: string | undefined): Promise<R> {
        let url = "/api/user/logout";
        let par = {
            token: token,
        };

        return apiHelper.request(url, par);
    }
}