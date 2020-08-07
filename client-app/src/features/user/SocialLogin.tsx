import React from "react";
import FacebookLogin from "react-facebook-login/dist/facebook-login-render-props";
import { Button, Icon } from "semantic-ui-react";
import { observer } from "mobx-react-lite";

interface Iprops {
  fbCallback: (response: any) => void;
  loading: boolean;
}

export const SocialLogin: React.FC<Iprops> = ({ fbCallback, loading }) => {
  return (
    <div>
      <FacebookLogin
        appId="482925899032031"
        fields="name,email,picture"
        callback={fbCallback}
        render={(renderProps: any) => (
          <Button
            loading={loading}
            onClick={renderProps.onClick}
            type="button"
            fluid
            color="facebook"
          >
            <Icon name="facebook" />
            Login with Facebook
          </Button>
        )}
      />
    </div>
  );
};

export default observer(SocialLogin);
