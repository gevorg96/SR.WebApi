// @flow
import React from 'react';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { store, persistor } from './store';
import { ToastContainer } from "react-toastify";

import './assets/_scss/main.scss';

import Routes from './routes/index';

const App = () => [
  <Provider store={store} key="Provider">
    <PersistGate loading={null} persistor={persistor}>
      <Routes />
    </PersistGate>
  </Provider>,
  <ToastContainer
    key="ToastContainer"
    autoClose={3000}
  />
];

export default App;
