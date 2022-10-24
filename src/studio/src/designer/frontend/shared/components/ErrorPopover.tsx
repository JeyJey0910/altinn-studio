import React from 'react';
import { createTheme, Grid, Typography } from '@mui/material';
import { createStyles, makeStyles } from '@mui/styles';
import AltinnPopover from './molecules/AltinnPopoverSimple';
import altinnTheme from '../theme/altinnStudioTheme';

export interface IErrorPopoverProps {
  anchorEl: Element | ((element: Element) => Element);
  onClose: (
    event: Record<string, unknown>,
    reason: 'backdropClick' | 'escapeKeyDown',
  ) => void;
  errorMessage: string;
}

const theme = createTheme(altinnTheme);

const useStyles = makeStyles(() =>
  createStyles({
    popoverRoot: {
      backgroundColor: theme.altinnPalette.primary.redLight,
    },
    errorIcon: {
      color: theme.altinnPalette.primary.red,
      fontSize: '36px',
    },
    errorText: {
      fontSize: '16px',
    },
  }),
);

export default function ErrorPopover({
  anchorEl,
  onClose,
  errorMessage,
}: IErrorPopoverProps) {
  const classes = useStyles();

  return (
    <AltinnPopover
      open={!!anchorEl}
      anchorEl={anchorEl}
      handleClose={onClose}
      anchorOrigin={{
        vertical: 'bottom',
        horizontal: 'left',
      }}
      paperProps={{
        classes: {
          root: classes.popoverRoot,
        },
      }}
    >
      <Grid container={true} direction='row' spacing={3} alignItems='center'>
        <Grid
          item={true}
          xs={1}
          style={{
            padding: 0,
          }}
        >
          <i className={`${classes.errorIcon} ai ai-circle-exclamation`} />
        </Grid>
        <Grid
          item={true}
          xs={11}
          style={{
            padding: 0,
          }}
        >
          <Typography className={classes.errorText}>{errorMessage}</Typography>
        </Grid>
      </Grid>
    </AltinnPopover>
  );
}
