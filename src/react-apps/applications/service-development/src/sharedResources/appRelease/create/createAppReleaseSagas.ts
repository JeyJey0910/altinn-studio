import { SagaIterator } from 'redux-saga';
import { call, fork, takeLatest } from 'redux-saga/effects';
import * as AppReleaseActionTypes from './../appReleaseActionTypes';
import AppReleaseActionDispatcher from './../appReleaseDispatcher';

function* createReleaseSaga(): SagaIterator {
  try {
    yield call(AppReleaseActionDispatcher.createReleaseFulfilled, 'release-id');
  } catch (err) {
    yield call(AppReleaseActionDispatcher.createReleaseRejected, err);
  }
}

export function* watchCreateReleaseSaga(): SagaIterator {
  yield takeLatest(
    AppReleaseActionTypes.CREATE_APP_RELEASE,
    createReleaseSaga,
  );
}

export default function*(): SagaIterator {
  yield fork(watchCreateReleaseSaga);
}
