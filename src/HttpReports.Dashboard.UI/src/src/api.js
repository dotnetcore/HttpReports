import axios from "axios";
import store from "./store/index";

export default {
  async getIndexChartData () {
    return await this.postJson(this.getFullAddress("/GetIndexChartData"),
      {
        "isDesc": true
      })
  },
  async getLocalizeLanguage (language) {
    return await this.get(this.getFullAddress(`/getLocalizeLanguage?language=${language}`));
  },
  async getAvailableLanguages () {
    return await this.get(this.getFullAddress(`/getAvailableLanguages`));
  },
  getFullAddress (path) {
    return store.getters.apiFullAddress(path);
  },
  async get (url) {
    return await axios.get(url);
  },
  async postJson (url, data) {
    return await axios.post(url, data,
      { headers: { 'Content-Type': 'application/json' } }
    );
  }
}
