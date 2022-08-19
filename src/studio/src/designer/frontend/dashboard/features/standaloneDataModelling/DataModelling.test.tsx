import React from 'react';
import ReactRouter from 'react-router';
import { Provider } from 'react-redux';
import configureStore from 'redux-mock-store';
import DataModelling from './DataModelling';
import { LoadingState } from 'app-shared/features/dataModelling/sagas/metadata';

import { render as rtlRender } from '@testing-library/react';

describe('DataModelling', () => {
  const language = { administration: {} };
  const modelName = 'model-name';
  const initialState = {
    dataModelsMetadataState: {
      dataModelsMetadata: [
        {
          repositoryRelativeUrl: `/path/to/models/${modelName}.schema.json`,
          fileName: `${modelName}.schema.json`,
          fileType: '.json',
        },
      ],
      loadState: LoadingState.ModelsLoaded,
    },
    dataModelling: {
      schema: {},
      saving: false,
    },
  };

  let store: any;
  const dispatchMock = () => Promise.resolve({});
  const initialStoreCall = {
    type: 'dataModelling/fetchDataModel',
    payload: {
      metadata: {
        label: modelName,
        value: initialState.dataModelsMetadataState.dataModelsMetadata[0],
      },
    },
  };

  beforeEach(() => {
    jest.spyOn(ReactRouter, 'useParams').mockReturnValue({
      org: 'test-org',
      repoName: 'test-repo',
    });

    store = configureStore()({ ...initialState, language: { language } });
    store.dispatch = jest.fn(dispatchMock);
  });

  it('should fetch models on mount', () => {
    rtlRender(
      <Provider store={store}>
        <DataModelling language={language} />
      </Provider>,
    );

    expect(store.dispatch).toHaveBeenCalledWith(initialStoreCall);
  });
});
