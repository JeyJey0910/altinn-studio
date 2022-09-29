import React, { ChangeEvent, MouseEvent, SyntheticEvent, useEffect, useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { TabContext, TabList, TabPanel } from '@material-ui/lab';
import { useDispatch, useSelector } from 'react-redux';
import { AppBar, Button, Typography } from '@material-ui/core';
import { AltinnMenu, AltinnMenuItem, AltinnSpinner } from 'app-shared/components';
import type { IJsonSchema, ILanguage, ISchemaState } from '../types';

import {
  addRootItem,
  setJsonSchema,
  setSchemaName,
  setSelectedTab,
  setUiSchema,
  updateJsonSchema,
} from '../features/editor/schemaEditorSlice';
import { getTranslation } from '../utils/language';
import { SchemaInspector } from './SchemaInspector';
import { SchemaTab } from './common/SchemaTab';
import { TopToolbar } from './TopToolbar';
import { getSchemaSettings } from '../settings';
import { getLanguageFromKey } from 'app-shared/utils/language';
import { SchemaTreeView } from './TreeView/SchemaTreeView';
import {
  CombinationKind,
  FieldType,
  getNodeByPointer,
  getNodeIndexByPointer,
  getRootNodes,
  ObjectKind,
  pointerExists,
  UiSchemaNode,
} from '@altinn/schema-model';

const useStyles = makeStyles({
  root: {
    height: 'calc(100vh - 111px)',
    width: '100%',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden',
    '& > main': {
      display: 'flex',
      flexDirection: 'row',
      flexGrow: 1,
      alignItems: 'stretch',
      minHeight: 0,
    },
  },
  editor: {
    backgroundColor: 'white',
    minHeight: 200,
    flexGrow: 1,
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
  },
  appBar: {
    borderBottom: '1px solid #C9C9C9',
    boxShadow: 'none',
    backgroundColor: '#fff',
    color: '#000',
    paddingLeft: 12,
    '& .Mui-Selected': {
      color: '#6A6A6A',
    },
    '& .MuiTabs-indicator': {
      backgroundColor: '#006BD8',
    },
  },
  tabPanel: {
    overflowY: 'scroll',
    flexGrow: 1,
  },
  inspector: {
    overflowX: 'clip',
    overflowY: 'auto',
    backgroundColor: '#F7F7F7',
  },
  addButton: {
    border: '1px dashed rgba(0, 0, 0, 1)',
    borderRadius: '0px',
    '&:hover': {
      border: '1px solid rgba(0, 0, 0, 1)',
    },
    textTransform: 'none',
    color: 'black',
    '& > i': {
      fontSize: '24px',
    },
    marginBottom: '12px',
  },
  startIcon: {
    marginRight: '0px',
  },
});

export interface IEditorProps {
  Toolbar: JSX.Element;
  LandingPagePanel: JSX.Element;
  language: ILanguage;
  loading?: boolean;
  name?: string;
  onSaveSchema: (payload: any) => void;
  schema: IJsonSchema;
}

export const SchemaEditor = ({
  Toolbar,
  LandingPagePanel,
  loading,
  schema,
  onSaveSchema,
  name,
  language,
}: IEditorProps) => {
  const classes = useStyles();
  const dispatch = useDispatch();

  const jsonSchema = useSelector((state: ISchemaState) => state.schema);
  const selectedPropertyNode = useSelector((state: ISchemaState) => state.selectedPropertyNodeId);
  const selectedDefinitionNode = useSelector((state: ISchemaState) => state.selectedDefinitionNodeId);

  const schemaSettings = getSchemaSettings({ schemaUrl: jsonSchema?.$schema });
  const { definitions, modelView } = useSelector((state: ISchemaState) => ({
    definitions: getRootNodes(state.uiSchema, true),
    modelView: getRootNodes(state.uiSchema, false),
  }));

  const selectedTab: string = useSelector((state: ISchemaState) => state.selectedEditorTab);
  const [expandedPropNodes, setExpandedPropNodes] = useState<string[]>([]);
  const [expandedDefNodes, setExpandedDefNodes] = useState<string[]>([]);
  const [menuAnchorEl, setMenuAnchorEl] = useState<null | Element>(null);
  const [editMode, setEditMode] = useState(false);

  const saveSchema = () => dispatch(updateJsonSchema({ onSaveSchema }));

  useEffect(() => {
    if (name) {
      dispatch(setSchemaName({ name }));
    }
  }, [dispatch, name]);

  useEffect(() => {
    if (jsonSchema && name) {
      dispatch(setUiSchema({ name }));
    }
  }, [dispatch, jsonSchema, name]);

  useEffect(() => {
    dispatch(setJsonSchema({ schema }));
  }, [dispatch, schema]);

  const openMenu = (e: MouseEvent) => {
    e.stopPropagation();
    setMenuAnchorEl(e.currentTarget);
  };

  const closeMenu = (e: SyntheticEvent) => {
    e.stopPropagation();
    setMenuAnchorEl(null);
  };

  const handleAddProperty = (objectKind: ObjectKind) => {
    const newNode: Partial<UiSchemaNode> = {
      objectKind,
    };
    if (objectKind === ObjectKind.Field) {
      newNode.fieldType = FieldType.Object;
    }
    if (objectKind === ObjectKind.Combination) {
      newNode.fieldType = CombinationKind.AllOf;
    }
    newNode.ref = objectKind === ObjectKind.Reference ? '' : undefined;
    dispatch(
      addRootItem({
        name: 'name',
        location: schemaSettings.propertiesPath,
        props: newNode,
      }),
    );

    setMenuAnchorEl(null);
  };

  const toggleEditMode = () => setEditMode((prevState) => !prevState);

  const handleAddDefinition = (e: MouseEvent) => {
    e.stopPropagation();
    dispatch(
      addRootItem({
        name: 'name',
        location: schemaSettings.definitionsPath,
        props: { fieldType: FieldType.Object },
      }),
    );
  };

  const handlePropertiesNodeExpanded = (_x: ChangeEvent<unknown>, nodeIds: string[]) => setExpandedPropNodes(nodeIds);

  const handleDefinitionsNodeExpanded = (_x: ChangeEvent<unknown>, nodeIds: string[]) => setExpandedDefNodes(nodeIds);

  const handleTabChanged = (_x: ChangeEvent<unknown>, value: 'definitions' | 'properties') =>
    dispatch(setSelectedTab({ selectedTab: value }));

  const loadingIndicator = loading ? (
    <AltinnSpinner spinnerText={getLanguageFromKey('general.loading', language)} />
  ) : null;

  const selectedId = useSelector((state: ISchemaState) =>
    state.selectedEditorTab === 'properties' ? state.selectedPropertyNodeId : state.selectedDefinitionNodeId,
  );

  const selectedItem = useSelector((state: ISchemaState) =>
    selectedId ? getNodeByPointer(state.uiSchema, selectedId) : null,
  );
  const uiSchema = useSelector((state: ISchemaState) => state.uiSchema);
  // if item is a reference, we want to show the properties of the reference.
  const referredItem = useSelector((state: ISchemaState) => {
    const index = getNodeIndexByPointer(state.uiSchema, selectedItem?.ref ?? '');
    return index !== undefined ? state.uiSchema[index] : undefined;
  });
  const checkIsNameInUse = (name: string) => pointerExists(uiSchema, name);
  const t = (key: string) => getTranslation(key, language);
  return (
    <div className={classes.root}>
      <TopToolbar
        Toolbar={Toolbar}
        language={language}
        saveAction={name ? saveSchema : undefined}
        toggleEditMode={name ? toggleEditMode : undefined}
        editMode={editMode}
      />
      <main>
        {LandingPagePanel}
        {name && schema ? (
          <div data-testid='schema-editor' id='schema-editor' className={classes.editor}>
            <TabContext value={selectedTab}>
              <AppBar position='static' color='default' className={classes.appBar}>
                <TabList onChange={handleTabChanged} aria-label='model-tabs'>
                  <SchemaTab label={t('model')} value='properties' />
                  <SchemaTab label={t('types')} value='definitions' />
                </TabList>
              </AppBar>
              <TabPanel className={classes.tabPanel} value='properties'>
                {editMode && (
                  <Button
                    endIcon={<i className='fa fa-drop-down' />}
                    onClick={openMenu}
                    className={classes.addButton}
                    id='add-button'
                  >
                    <Typography variant='body1'>{t('add')}</Typography>
                  </Button>
                )}
                <SchemaTreeView
                  editMode={editMode}
                  expanded={expandedPropNodes}
                  items={modelView}
                  translate={t}
                  onNodeToggle={handlePropertiesNodeExpanded}
                  selectedPointer={selectedPropertyNode}
                />
              </TabPanel>
              <TabPanel className={classes.tabPanel} value='definitions'>
                {editMode && (
                  <Button
                    startIcon={<i className='fa fa-plus' />}
                    onClick={handleAddDefinition}
                    className={classes.addButton}
                    classes={{ startIcon: classes.startIcon }}
                  >
                    <Typography variant='body1'>{t('add_element')}</Typography>
                  </Button>
                )}
                <SchemaTreeView
                  editMode={editMode}
                  expanded={expandedDefNodes}
                  items={definitions}
                  translate={t}
                  onNodeToggle={handleDefinitionsNodeExpanded}
                  selectedPointer={selectedDefinitionNode}
                />
              </TabPanel>
            </TabContext>
          </div>
        ) : (
          loadingIndicator
        )}
        {schema && editMode && (
          <aside className={classes.inspector}>
            <SchemaInspector
              language={language}
              referredItem={referredItem ?? undefined}
              selectedItem={selectedItem ?? undefined}
              checkIsNameInUse={checkIsNameInUse}
            />
          </aside>
        )}
      </main>
      <AltinnMenu anchorEl={menuAnchorEl} open={Boolean(menuAnchorEl)} onClose={closeMenu}>
        <AltinnMenuItem
          onClick={() => handleAddProperty(ObjectKind.Field)}
          text={t('field')}
          iconClass='fa fa-datamodel-properties'
          id='add-field-button'
        />
        <AltinnMenuItem
          onClick={() => handleAddProperty(ObjectKind.Reference)}
          text={t('reference')}
          iconClass='fa fa-datamodel-ref'
          id='add-reference-button'
        />
        <AltinnMenuItem
          onClick={() => handleAddProperty(ObjectKind.Combination)}
          text={t('combination')}
          iconClass='fa fa-group'
          id='add-combination-button'
        />
      </AltinnMenu>
    </div>
  );
};
