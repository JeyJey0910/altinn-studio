import React, { useEffect, useState } from 'react';
import { DndProvider } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';
import { useDispatch, useSelector } from 'react-redux';
import FileEditor from 'app-shared/file-editor/FileEditor';
import { RightMenu } from '../components/rightMenu/RightMenu';
import { filterDataModelForIntellisense } from '../utils/datamodel';
import { DesignView } from './DesignView';
import { fetchServiceConfiguration } from '../features/serviceConfigurations/serviceConfigurationSlice';
import { FormLayoutActions } from '../features/formDesigner/formLayout/formLayoutSlice';
import type { IAppState, IDataModelFieldElement, LogicMode } from '../types/global';
import { makeGetLayoutOrderSelector } from '../selectors/getLayoutData';
import { deepCopy } from 'app-shared/pure';
import classes from './FormDesigner.module.css';
import { LeftMenu } from '../components/leftMenu/LeftMenu';
import { Warning } from '@navikt/ds-icons';

export const FormDesigner = (): JSX.Element =>  {
  const dispatch = useDispatch();

  const [codeEditorOpen, setCodeEditorOpen] = useState<boolean>(false);
  const [codeEditorMode, setCodeEditorMode] = useState<LogicMode>(null);

  const selectedLayout: string = useSelector(
    (state: IAppState) => state.formDesigner.layout.selectedLayout
  );
  const dataModel = useSelector((state: IAppState) => state.appData.dataModel.model);

  useEffect(() => {
    dispatch(FormLayoutActions.fetchFormLayout());
    dispatch(fetchServiceConfiguration());
  }, [dispatch]);

  const toggleCodeEditor = (mode?: LogicMode) => {
    setCodeEditorOpen(!codeEditorOpen);
    setCodeEditorMode(mode || null);
  };

  const getDataModelSuggestions = (filterText: string): IDataModelFieldElement[] => {
    return filterDataModelForIntellisense(dataModel, filterText);
  };

  const getEditorHeight = () => {
    const height = document.getElementById('formFillerGrid').clientHeight;
    const editorHeight = height - 20;
    return editorHeight.toString();
  };

  const renderLogicEditor = () => {
    return (
      <div className={classes.logicEditor}>
        <div>
          <FileEditor
            editorHeight={getEditorHeight()}
            mode={codeEditorMode.toString()}
            closeFileEditor={toggleCodeEditor}
            getDataModelSuggestions={getDataModelSuggestions}
            boxShadow={true}
          />
        </div>
      </div>
    );
  };

  const activeList = useSelector((state: IAppState) => state.formDesigner.layout.activeList);
  const layoutOrder = useSelector((state: IAppState) =>
    deepCopy(
      state.formDesigner.layout.layouts[state.formDesigner.layout.selectedLayout]?.order || {}
    )
  );

  const order = useSelector(makeGetLayoutOrderSelector());

  return (
    <DndProvider backend={HTML5Backend}>
      <div className={classes.root}>
        <div className={classes.container} id='formFillerGrid'>
          <div className={classes.leftContent + ' ' + classes.item}>
            <LeftMenu/>
          </div>
          <div className={classes.mainContent + ' ' + classes.item}>
            <h1 className={classes.pageHeader}>{selectedLayout}</h1>
            {selectedLayout === 'Kvittering' && (
              <p className={classes.warningMessage}>
                <Warning/> Denne funksjonaliteten er enn?? ikke implementert i appene.
              </p>
            )}
            <DesignView
              order={order}
              activeList={activeList}
              isDragging={false}
              layoutOrder={layoutOrder}
            />
            {codeEditorOpen ? renderLogicEditor() : null}
          </div>
          <div className={classes.rightContent + ' ' + classes.item}>
            <RightMenu toggleFileEditor={toggleCodeEditor} />
          </div>
        </div>
      </div>
    </DndProvider>
  );
}
