import React, { useEffect } from 'react';
import {
  CircularProgress,
  Grid,
  Popover,
  Typography,
} from '@mui/material';
import AltinnIcon from 'app-shared/components/AltinnIcon';
import { getLanguageFromKey, getParsedLanguageFromKey } from 'app-shared/utils/language';
import type { IAppReleaseState } from '../../../sharedResources/appRelease/appReleaseSlice';
import { AppReleaseActions } from '../../../sharedResources/appRelease/appReleaseSlice';
import type { IRelease } from '../../../sharedResources/appRelease/types';
import { BuildResult, BuildStatus } from '../../../sharedResources/appRelease/types';
import type { IRepoStatusState } from '../../../sharedResources/repoStatus/repoStatusSlice';
import { RepoStatusActions } from '../../../sharedResources/repoStatus/repoStatusSlice';
import { fetchLanguage } from '../../../utils/fetchLanguage/languageSlice';
import type { IHandleMergeConflictState } from '../../handleMergeConflict/handleMergeConflictSlice';
import { fetchRepoStatus } from '../../handleMergeConflict/handleMergeConflictSlice';
import ReleaseComponent from '../components/appReleaseComponent';
import CreateReleaseComponent from '../components/createAppReleaseComponent';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector, useMediaQuery } from '../../../common/hooks';
import { frontendLangPath, gitCommitPath, repoStatusPath } from 'app-shared/api-paths';
import classes from './releaseContainer.module.css';

function AppReleaseContainer() {
  const hiddenMdDown = useMediaQuery('(max-width: 1025px)');
  const dispatch = useAppDispatch();

  const [anchorElement, setAchorElement] = React.useState<Element>();

  const [popoverOpenClick, setPopoverOpenClick] = React.useState<boolean>(false);
  const [popoverOpenHover, setPopoverOpenHover] = React.useState<boolean>(false);

  const appReleases: IAppReleaseState = useAppSelector((state) => state.appReleases);
  const repoStatus: IRepoStatusState = useAppSelector((state) => state.repoStatus);
  const handleMergeConflict: IHandleMergeConflictState = useAppSelector(
    (state) => state.handleMergeConflict
  );
  const language: any = useAppSelector((state) => state.languageState.language);
  const { org, app } = useParams();

  useEffect(() => {
    dispatch(AppReleaseActions.getAppReleaseStartInterval());
    if (!language) {
      dispatch(fetchLanguage({ url: frontendLangPath('nb') }));
    }
    dispatch(RepoStatusActions.getMasterRepoStatus({ org, repo: app }));
    dispatch(
      fetchRepoStatus({
        url: repoStatusPath(org, app),
        org,
        repo: app,
      })
    );
    return () => {
      dispatch(AppReleaseActions.getAppReleaseStopInterval());
    };
  }, [dispatch, language, org, app]);

  function handlePopoverKeyPress(event: React.KeyboardEvent) {
    if (event.key === 'Enter' || event.key === ' ') {
      if (!anchorElement) {
        setAchorElement(event.currentTarget);
      }
      setPopoverOpenClick(!popoverOpenClick);
    }
  }

  function handlePopoverOpenClicked(event: React.MouseEvent) {
    if (!anchorElement) {
      setAchorElement(event.currentTarget);
    }
    setPopoverOpenClick(!popoverOpenClick);
  }

  function handlePopoverOpenHover(event: React.MouseEvent) {
    setAchorElement(event.currentTarget);
    setPopoverOpenHover(true);
  }

  function handlePopoverClose() {
    if (popoverOpenHover) {
      setPopoverOpenHover(!popoverOpenHover);
    }
  }

  function renderCannotCreateRelease() {
    return (
      <Grid
        container={true}
        direction='row'
        className={classes.cannotCreateReleaseContainer}
        spacing={1}
      >
        {hiddenMdDown ? null : (
          <Grid item={true} xs={1}>
            <AltinnIcon
              iconClass={`${classes.renderCannotCreateReleaseIcon} ai ai-circle-exclamation`}
              iconColor='#E23B53'
            />
          </Grid>
        )}
        <Grid item={true} xs={12} md={10}>
          <Grid container={true} direction='column'>
            <Typography className={classes.cannotCreateReleaseTitle}>
              {getParsedLanguageFromKey(
                'app_create_release_errors.fetch_release_failed',
                language,
                ['mailto:tjenesteeier@altinn.no']
              )}
            </Typography>
            <Typography className={classes.cannotCreateReleaseSubTitle}>
              {getLanguageFromKey('app_create_release_errors.technical_error_code', language)}
              &nbsp;
              {appReleases.errors.fetchReleaseErrorCode}
            </Typography>
          </Grid>
        </Grid>
      </Grid>
    );
  }

  function renderCreateRelease() {
    if (appReleases.errors.fetchReleaseErrorCode !== null) {
      return renderCannotCreateRelease();
    }
    if (!repoStatus.branch.master || !handleMergeConflict.repoStatus.contentStatus) {
      return (
        <Grid container={true} direction='row' justifyContent='center'>
          <Grid container={true} direction='row' justifyContent='center'>
            <Grid
              container={true}
              direction='column'
              justifyContent='space-evenly'
              style={{
                padding: '2rem',
              }}
            >
              <Grid item={true}>
                <CircularProgress
                  style={{
                    color: '#1EAEF7',
                  }}
                />
              </Grid>
              <Grid item={true}>
                <Typography
                  style={{
                    padding: '1.2rem',
                  }}
                >
                  {getLanguageFromKey('app_create_release.check_status', language)}
                </Typography>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      );
    }
    if (appReleases.errors.fetchReleaseErrorCode !== null) {
      return null;
    }
    if (!appReleases.releases || !appReleases.releases.length) {
      return <CreateReleaseComponent />;
    }
    if (!handleMergeConflict.repoStatus || !repoStatus.branch.master) {
      return null;
    }
    // Check if latest
    if (
      !!appReleases.releases[0] &&
      appReleases.releases[0].targetCommitish === repoStatus.branch.master.commit.id &&
      appReleases.releases[0].build.status === BuildStatus.completed &&
      appReleases.releases[0].build.result === BuildResult.succeeded
    ) {
      return null;
    }
    if (appReleases.releases[0].build.status !== BuildStatus.completed) {
      return null;
    }
    return <CreateReleaseComponent />;
  }

  function renderStatusIcon() {
    if (
      !repoStatus.branch.master ||
      !handleMergeConflict.repoStatus.contentStatus ||
      !handleMergeConflict.repoStatus.contentStatus.length ||
      !appReleases.releases.length
    ) {
      return null;
    }
    if (
      !!handleMergeConflict.repoStatus.contentStatus ||
      !!handleMergeConflict.repoStatus.aheadBy
    ) {
      return <i className='fa fa-circle-upload' />;
    }
    return null;
  }

  function renderStatusMessage() {
    if (
      !repoStatus.branch.master ||
      !appReleases.releases ||
      !handleMergeConflict.repoStatus.contentStatus
    ) {
      return null;
    }
    if (!appReleases.releases || !appReleases.releases.length) {
      return null;
    }
    if (
      !!appReleases.releases[0] &&
      repoStatus.branch.master.commit.id === appReleases.releases[0].targetCommitish
    ) {
      return (
        <Typography>
          {getLanguageFromKey('app_create_release.local_changes_cant_build', language)}
        </Typography>
      );
    }
    if (handleMergeConflict.repoStatus.contentStatus) {
      return (
        <Typography>
          {getLanguageFromKey('app_create_release.local_changes_can_build', language)}
        </Typography>
      );
    }
    return null;
  }

  function renderCreateReleaseTitle() {
    if (
      !!appReleases.errors.fetchReleaseErrorCode ||
      !repoStatus.branch.master ||
      !handleMergeConflict.repoStatus.contentStatus
    ) {
      return null;
    }
    const latestRelease: IRelease = appReleases.releases[0] ? appReleases.releases[0] : null;
    if (
      !latestRelease ||
      latestRelease.targetCommitish !== repoStatus.branch.master.commit.id ||
      !handleMergeConflict.repoStatus.contentStatus
    ) {
      return (
        <Typography>
          {getLanguageFromKey('app_release.release_title', language)} &nbsp;
          {repoStatus.branch.master ? (
            <a
              href={gitCommitPath(org, app, repoStatus.branch.master.commit.id)}
              target='_blank'
              rel='noopener noreferrer'
            >
              {getLanguageFromKey('app_release.release_title_link', language)}
            </a>
          ) : null}
        </Typography>
      );
    }
    if (latestRelease.targetCommitish === repoStatus.branch.master.commit.id) {
      return (
        <>
          {getLanguageFromKey('general.version', language)}
          &nbsp;
          {appReleases.releases[0].tagName}
          &nbsp;
          {getLanguageFromKey('general.contains', language)}
          &nbsp;
          <a href={gitCommitPath(org, app, repoStatus.branch.master.commit.id)}>
            {getLanguageFromKey('app_release.release_title_link', language)}
          </a>
        </>
      );
    }
    return null;
  }

  return (
    <>
      <Grid container={true} direction='row' className={classes.appReleaseWrapper}>
        <Grid container={true} direction='column'>
          <Grid item={true} className={classes.versionHeader}>
            <Typography className={classes.versionHeaderTitle}>
              {getLanguageFromKey('app_release.release_tab_versions', language)}
            </Typography>
          </Grid>

          <Grid container={true} direction='column' className={classes.appCreateReleaseWrapper}>
            <Grid container={true} direction='row' justifyContent='space-between'>
              <Grid item={true} xs={10}>
                <Typography className={classes.appCreateReleaseTitle}>
                  {renderCreateReleaseTitle()}
                </Typography>
              </Grid>
              <Grid
                item={true}
                className={classes.appCreateReleaseStatusIcon}
                onClick={handlePopoverOpenClicked}
                onMouseOver={handlePopoverOpenHover}
                onMouseLeave={handlePopoverClose}
                tabIndex={0}
                onKeyPress={handlePopoverKeyPress}
                xs={1}
              >
                {renderStatusIcon()}
              </Grid>
            </Grid>
            <Grid item={true} className={classes.appReleaseCreateRelease}>
              {renderCreateRelease()}
            </Grid>
          </Grid>
          <Grid item={true}>
            <Typography className={classes.appReleaseHistoryTitle}>
              {getLanguageFromKey('app_release.earlier_releases', language)}
            </Typography>
          </Grid>
          <Grid container={true} className={classes.appReleaseHistory}>
            {!!appReleases.releases.length &&
              appReleases.releases.map((release: IRelease, index: number) => (
                <ReleaseComponent key={index} release={release} />
              ))}
          </Grid>
        </Grid>
      </Grid>
      <Popover
        className={classes.popover}
        classes={{
          paper: classes.popoverPaper,
        }}
        anchorEl={anchorElement}
        open={popoverOpenClick || popoverOpenHover}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left',
        }}
        onClose={handlePopoverClose}
      >
        {renderStatusMessage()}
      </Popover>
    </>
  );
}

export default AppReleaseContainer;
