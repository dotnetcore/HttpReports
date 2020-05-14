import axios from "axios";
import store from "./store/index";

export default {
  async getIndexChartData () {
    return await axios.post(store.getters.apiFullAddress("/GetIndexChartData"),
      {
        "isDesc": true
      },
      { headers: { 'Content-Type': 'application/json' } }
    );
  }
}
