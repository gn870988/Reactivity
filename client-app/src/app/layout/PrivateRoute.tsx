import React, { useContext } from "react";
import {
  RouteProps,
  RouteComponentProps,
  Route,
  Redirect
} from "react-router-dom";
import { RootStoreContext } from "../stores/rootStore";
import { observer } from "mobx-react-lite";

interface Iprops extends RouteProps {
  component: React.ComponentType<RouteComponentProps<any>>;
}

export const PrivateRoute: React.FC<Iprops> = ({
  component: Component,
  ...rest
}) => {
  const rootStore = useContext(RootStoreContext);
  const { isLoggedIn } = rootStore.userStore;
  return (
    <Route
      {...rest}
      render={props =>
        isLoggedIn ? <Component {...props} /> : <Redirect to={"/"} />
      }
    />
  );
};

export default observer(PrivateRoute);
